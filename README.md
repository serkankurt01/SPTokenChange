## ADFS/Azure JWT Token For SharePoint On-Premises RestApi

Azure veya ADFS üzerinden aldığınız Access Token ile SharePoint REST Apilerine istek yapabilmenize olanak sağlayan bir uygulamadır. 

SharePoint’e ait özelliklerden bir tanesi olan REST Api alt yapısını kullarak uygulama içerisindeki içeriklere erişme ihtiyacı doğmuştur. Bu noktada SharePoint’e dışarıdan erişim için sağlayabilmek için aşağıdaki gibi bir çok Authentication seçeneği bulunmaktadır.  

- Basic Authentication 
- NTLM Auth 
- OAuth2  
- Federation Auth  

Bu seçeneklerin her biri tek başına ihtiyacı karşılamamaktadır. Bu yüzden bu dokümanında atlattığı gibi bu seçenklerin bir kombinasyonu ile ihtiyacımız olan erişim seçeneğine ulaşmış olacağız.  
Yapılan kurguda iki farklı token mekanizması bulunmaktadır. Birinci ADFS tarafında yönetilen Access Token süreci diğeri ise SharePoint içerisinde yönetilen OAuth2 Access Token sürecidir. 


Söz konusu çalışmayı sağlayabilmek için bir HTTP Module oluşturulmuştur.  Bu module ADFS ile SharePoint arasıda bir köprü görevi görmektedir.  Süreç özet olarak aşağıdaki şekilde ilerlemektedir.  

- ADFS üzerinden kullanıcı girişi yapılarak bir Access Token elde edilir. 
- SharePoint rest isteklerinde yetkilendirme başlığı içerisine bu token eklenir. 
- HTTP Module bu tokeni yakalayarak ADFS üzerinden doğrulamasını gerçekleştirir.  
- HTTP Module tarafından doğrulanan token içerisindeki UPN adresi çözümlenir ve bu UPN ile SharePoint oAuth2 model kullanılarak yeni bir access token oluşturulur. 
- Oluşturulan Access token yapılan Rest isteğinin yetkilendirme başlığına eklenerek süreç tamamlanır.  

## ADFS
