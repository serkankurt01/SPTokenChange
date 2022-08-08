## ADFS/Azure JWT Token For SharePoint On-Premises RestApi

Azure veya ADFS üzerinden aldığınız Access Token ile SharePoint REST Apilerine istek yapabilmenize olanak sağlayan bir uygulamadır. 

SharePoint’e ait özelliklerden bir tanesi olan REST Api alt yapısını kullarak uygulama içerisindeki içeriklere erişme ihtiyacı doğmuştur. Bu noktada SharePoint’e dışarıdan erişim için sağlayabilmek için aşağıdaki gibi bir çok Authentication seçeneği bulunmaktadır.  

- Basic Authentication 
- NTLM Auth 
- OAuth2  
- Federation Auth  

Bu seçeneklerin her biri tek başına ihtiyacı karşılamamaktadır. Bu yüzden bu dokümanında atlattığı gibi bu seçenklerin bir kombinasyonu ile ihtiyacımız olan erişim seçeneğine ulaşmış olacağız.  
Yapılan kurguda iki farklı token mekanizması bulunmaktadır. Birinci ADFS tarafında yönetilen Access Token süreci diğeri ise SharePoint içerisinde yönetilen OAuth2 Access Token sürecidir. 

[![N|Solid](http://www.serkan-kurt.com.tr/blog/wp-content/uploads/2022/08/sptokenchange.png)]

Söz konusu çalışmayı sağlayabilmek için bir HTTP Module oluşturulmuştur.  Bu module ADFS ile SharePoint arasıda bir köprü görevi görmektedir.  Süreç özet olarak aşağıdaki şekilde ilerlemektedir.  

- ADFS üzerinden kullanıcı girişi yapılarak bir Access Token elde edilir. 
- SharePoint rest isteklerinde yetkilendirme başlığı içerisine bu token eklenir. 
- HTTP Module bu tokeni yakalayarak ADFS üzerinden doğrulamasını gerçekleştirir.  
- HTTP Module tarafından doğrulanan token içerisindeki UPN adresi çözümlenir ve bu UPN ile SharePoint oAuth2 model kullanılarak yeni bir access token oluşturulur. 
- Oluşturulan Access token yapılan Rest isteğinin yetkilendirme başlığına eklenerek süreç tamamlanır.  

## ADFS

> Note: Bu madde içerisindeki scriptler, `ADFS` yüklü olduğu sunucu içersinde çalıştırılmalıdır.

```sh
cls
function JUNO-ADFSConfigs {
    Param(
        [Parameter(Mandatory = $true)][string]$name,
        [Parameter(Mandatory = $true)][string]$urn,
        [Parameter(Mandatory = $true)][string]$spWebUrl
    )

    $endpoint = $spWebUrl + "/_trust/"
    [string[]] $urnCollection = $urn, $spWebUrl, $endpoint
    $clietId = (New-Guid).ToString()
    $redirectUrl = "https://localhost"
    $clintName = $name + "ClientAPP"
    $clientAPP = Add-AdfsClient -Name $clintName -ClientId $clietId -RedirectUri $redirectUrl 
    $IssuanceAuthorizationRules = @'
@RuleTemplate = "AllowAllAuthzRule"
 => issue(Type = "http://schemas.microsoft.com/authorization/claims/permit",
Value = "true");
'@
    #Stores Issuance Transformation Rules
    $IssuanceTransformRules = @'
@RuleTemplate = "LdapClaims"
@RuleName = "ADClaims"
c:[Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname", Issuer == "AD AUTHORITY"]
 => issue(store = "Active Directory", types = ("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"), query = ";userPrincipalName,mail,sAMAccountName,tokenGroups;{0}", param = c.Value);
'@
 
    $rpt = Add-AdfsRelyingPartyTrust -Name $name -ProtocolProfile WSFederation -WSFedEndpoint $endpoint -Identifier $urnCollection -IssuanceTransformRules $IssuanceTransformRules -IssuanceAuthorizationRules $IssuanceAuthorizationRules
    $grand = Grant-AdfsApplicationPermission -ClientRoleIdentifier $clietId -ServerRoleIdentifier $urn  -ScopeNames openid, email, aza, user_impersonation, allatclaims 
    $iden = Get-AdfsProperties | select Identifier
    $protocol = Get-AdfsEndpoint | Where-Object { $_.Protocol -eq "OpenID Connect Discovery" }
    $signinUrl = Get-AdfsEndpoint | Where-Object { $_.Protocol -eq "SAML 2.0/WS-Federation" }

    write-host "STS Discovery Endpoint: "  $protocol.FullUrl -ForegroundColor Green
    write-host "Signin URL: "  $signinUrl.FullUrl -ForegroundColor Green
    write-host "Issuer : "  $iden.Identifier.AbsoluteUri -ForegroundColor Green
    write-host "Audiance : "  $clietId -ForegroundColor Green
    write-host "Client ID : "  $clietId -ForegroundColor Green

}

JUNO-ADFSConfigs -name "JunoTest6" -urn "urn:sharepoint:junowebtest6" -spWebUrl "https://jnn6.contoso.com" 

```
[![N|Solid](http://www.serkan-kurt.com.tr/blog/wp-content/uploads/2022/08/adfs-output.png)]
> Note: Yukarıdaki Script çalıştırıldıktan sonra ekranda çıkan parametrelerin not alınması gerekmektedir. 


Parametre Açıklamaları:

- name : 
    - Oluşturulacak olan ADFS uygulamasına ait tanımlayıcı bir isim
- 	Urn
    - 	Oluşturulacak ADFS uygulamasına ait benzersiz tanımlayıcı bilgi urn:sharepoint:{appname} formatında yazılmalıdır.
- spWebUrl 
    - 	Giriş yapılacak SharePoint Web Application adresi.

## SharePoint
### Trusted Security Token Issuer Oluşturma

> Note: Bu madde içerisindeki scriptler, `SharePoint` yüklü olduğu sunucu içersinde çalıştırılmalıdır.

> Note: Hali hazırda bir sertifika varsa ilgili sertifika kullanılabilir. Aksi durumda https://docs.microsoft.com/en-us/sharepoint/dev/sp-add-ins/create-high-trust-sharepoint-add-ins#obtain-a-certificate-or-create-a-public-and-private-test-certificate    adresinde ki makaleden faydalanarak ilgili sertifika oluşturularak export edilmelidir.

```sh
Add-PSSnapin "Microsoft.SharePoint.PowerShell"
function JUNO-SPConfigs {
    Param(
        [Parameter(Mandatory = $true)][string]$certificatePath,
        [Parameter(Mandatory = $true)][string]$spWebUrl,
        [Parameter(Mandatory = $true)][string]$clientAppName
    )
    $issuerID = (New-Guid).ToString()
    $targetSite = Get-SPSite $spWebUrl
    $realm = Get-SPAuthenticationRealm -ServiceContext $targetSite
    $registeredIssuerName = $issuerID + '@' + $realm

    $publicCertificate = Get-PfxCertificate $certificatePath
    $certificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($publicCertificate)
    try {

        $sptra = New-SPTrustedRootAuthority -Name $clientAppName -Certificate $certificate 
    }
    catch {}
    $serviceConfig1 = New-SPTrustedSecurityTokenIssuer -Name $issuerID  -RegisteredIssuerName $registeredIssuerName -Certificate $publicCertificate -IsTrustBroker
    $serviceConfig = Get-SPSecurityTokenServiceConfig
    $serviceConfig.AllowOAuthOverHttp = $true
    $serviceConfig.Update()

    $AppClientId = (New-Guid).ToString()
    $AppIdentifier = $AppClientId + "@" + $realm    
    
    $sites = Get-SPSite -WebApplication $spWebUrl -Limit All
    foreach($siteCol in $sites)
    {
        $regSPClient = Register-SPAppPrincipal -NameIdentifier $AppIdentifier -Site $siteCol.Url -DisplayName $clientAppName 
        $per = Set-SPAppPrincipalPermission -AppPrincipal $regSPClient -Site $siteCol.RootWeb -Scope SiteCollection -Right FullControl    
    }
    write-host "SP Issuer ID: "  $registeredIssuerName -ForegroundColor Green
    write-host "SP Client ID : "  $AppClientId -ForegroundColor Green
    write-host "SP Realm : "  $realm -ForegroundColor Green
}
JUNO-SPConfigs -clientAppName "JunoOpenIDConnect" -spWebUrl "https://jnn6.contoso.com" -certificatePath "D:\Migration\Cert\token-cert.cer" 
"

```
[![N|Solid](http://www.serkan-kurt.com.tr/blog/wp-content/uploads/2022/08/sp-output.png)]

Parametre Açıklamaları
- clientAppName 
  - Oluşturulacak application için tanımlayıcı bir isim verilmelidir.
- spWebUrl
  - Uygulamanın kullanılacağı Web Application adresi
- certificatePath
  - Note adımında bahsedilen sertifika. 


### Client Application Yetkilendirme
Powershell scripti çalıştırıldığıda ilgili web application altında bir uygulama oluşturacak ve Client ID bilgisi powershell ekranında görünecektir. 

Buradaki Client ID bilgisi kopyalandıktan sonra aşağıdaki “Central Admin” adresine girilmeli ve açılan sayfadaki bilgiler doldurularak “Create” butonuna tıklanmalıdır. http://{centraladminurl}/_layouts/15/appinv.aspx

[![N|Solid](http://www.serkan-kurt.com.tr/blog/wp-content/uploads/2022/08/appinv.png)]

Parametre Açıklamaları
- App Id
    - Bir önceki adımda not alınan SP Client ID parametresi bu alana girilerek yanında bulunan Lookup butonuna tıklanır. 
- Title
    - Değiştirilmemeli
- App Domain
    - Localhost yazılabilir
- Reditect Url
    - https://localhost yazılabilir
- Permission Request XML
    - Site Collection ve Tenant bazında full control yetkisi verilmelidir. Bunun için ilgii alana aşağıdaki xml yazılmalıdır
    
    ```sh
    <AppPermissionRequests>
      <AppPermissionRequest Scope="http://sharepoint/content/sitecollection" Right="FullControl"/>
      <AppPermissionRequest Scope="http://sharepoint/social/tenant" Right="FullControl"/>
    </AppPermissionRequests>

    ```
“Create” butonuna tıkladıktan sonra çıkan ektanda bulunan “Trust It” butonuna tıklanmalıdır. 

[![N|Solid](http://www.serkan-kurt.com.tr/blog/wp-content/uploads/2022/08/appint-trust.png)]


### SPTokenChange Konfigürasyonları

Tüm powershell scriptleri çalıştırıldıktan sonra repoda bulunan SharePoint projesi indirilerek WSP paketi oluşturulur.  Daha sonra ilgili wsp paketi farma deploy edilir.  

[![N|Solid](http://www.serkan-kurt.com.tr/blog/wp-content/uploads/2022/08/central-admin.png)]

Deploy işlemi tamamlandıktan sonra Central Administration -> Security altında “Juno Token Change” isminde yeni bir menu oluşacaktır. “Setup” linkine tıklayarak ilgili konfigürasyon ekranlarına ulaşılır.

Açılan sayfada üst bölümde bulunan web application seçim ekranından kullanılacak olan web application seçilir ve “Deploy” butonuna tıklanır.  Tıklama sonrası “Installede Applications” altında ki listeye ilgili web application eklenir. 

[![N|Solid](http://www.serkan-kurt.com.tr/blog/wp-content/uploads/2022/08/sptokenchange-admin.png)]

 Buradaki içerikler yukarıda çalıştırılan scriptler sonucu not alınan parametrelere göre doldurulurak Save butonuna tıklanır. 
 
Parametre Açıklamaları:

- User Profile Type
    - SharePoint tarafında ilgili kullanıcıyı bulmak için kullanılmaktadır.  Eğer Web sitesine ADFS veya Azure ile giriş yapılacaksa burada “Trusted Provider” seçeneği seçilmelidir.  Eğer web tarafı için herhangi bir trust işlemi yapılmayacaksa veya web tarafı Web Application Proxy ile trust edilecekse burada Windows seçeneği seçilmelidir. 
- Auth Type
    - Mobil uygulamanın kullandığı Auth Library tipi.
- JWT Token LifeTime Check
    - Gönderilen JWT token için süre validasyonu yapılıp yapılmayacağını belirler. Debug işlemleri için eklenmiştir. 
- Debug Mode
    - Debug işlemleri için eklenmiştir. Süreç içerisinde fazladan log kayıtları oluşturur. 
- Active
    - Debug işlemleri için eklenmiştir. IIS Module devreye girip girmeyeceğini belirler.
- Audience
    - ADFS Konfigürasyonları sonuçu ekranda oluşan bilgilendirme notları içerisindeki “Audience” seçeneği yazılmalıdır.  Bu değer aynı zamanda gönderilen JWT token içerisindeki “aud” değeri ile aynı olmalıdır. 
- Issuer
    - ADFS Konfigürasyonları sonuçu ekranda oluşan bilgilendirme notları içerisindeki “Issuer” seçeneği yazılmalıdır.  Bu değer aynı zamanda gönderilen JWT token içerisindeki “iss” değeri ile aynı olmalıdır. 
- STS Discovery EndPoint
    - JWT token oluşturan sağlayıcıya ait Discovery Endpoint adresi. ADFS Konfigürasyonları sonuçu ekranda oluşan bilgilendirme notları içerisindeki “STS Discovery Endpoint” seçeneği yazılmalıdır. 

- SP Client Id
    - SharePoint Konfigürasyonları sonuçu ekranda oluşan bilgilendirme notları içerisindeki “SP Client ID” seçeneği yazılmalıdır.  
- SP Issuer Id
    - SharePoint Konfigürasyonları sonuçu ekranda oluşan bilgilendirme notları içerisindeki “SP Issuer ID” seçeneği yazılmalıdır.  
- SP Realm
    - SharePoint Konfigürasyonları sonuçu ekranda oluşan bilgilendirme notları içerisindeki “SP Realm” seçeneği yazılmalıdır.  

- Auth Provider Name
    - Web tarafına ADFS ile giriş yapılacak ise trusted provider name yazılmalıdır. Örneğin; “trusted:LDAPCP”,
    - Web tarafında Azure ile giriş yapılacakise trusted provider name yazılmalıdır. Örneğin; “trusted:AzureCP”
    - Web tarafına NTML ile giriş yapılacak ise boş bırakılmalıdır. 
- Certifiate Path
    - SharePoint Konfigürasyonları adımında kullanılan sertifikaya ait PFX dosyasının path bilgisi
- Certificate Password
    - SharePoint Konfigürasyonları adımında kullanılan sertifikaya ait şifre

