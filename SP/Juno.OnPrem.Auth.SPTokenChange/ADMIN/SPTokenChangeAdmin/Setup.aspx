<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Setup.aspx.cs" Inherits="Juno.OnPrem.Auth.SPTokenChange.Layouts.Juno.OnPrem.Auth.SPTokenChange.Setup" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="/_admin/sptokenchangeadmin/assets/bootstrap/css/bootstrap.min.css" rel="stylesheet">
    <script src="/_admin/sptokenchangeadmin/assets/jquery-3.6.0.min.js" type="text/javascript"></script>
    <script src="/_admin/sptokenchangeadmin/assets/bootstrap/js/bootstrap.bundle.min.js"></script>

    <script src="/_admin/sptokenchangeadmin/assets/main.js" type="text/javascript"></script>
</asp:Content>


<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-2">
                <h5>Web Application</h5>
                Select a web application.
		   
            </div>
            <div class="col-6">
                <SharePoint:WebApplicationSelector AllowAdministrationWebApplication="false" UseDefaultSelection="false" runat="server" ID="WebAppSelector" OnContextChange="WebAppSelector_OnContextChange" />
            </div>
            <div class="col-2">
                <asp:Button runat="server" ID="btnUpdate" Text="Deploy" CssClass="btn btn-primary" OnClick="btnUpdateButton_Click" />
            </div>
        </div>
        <div class="row">
            <div class="col">
                <hr />
                <h4>Installed Applications</h4>
                <hr />
            </div>
        </div>
        <div class="row">
            <div class="col">
                <div class="accordion" id="accordionExample">

                    <asp:Repeater runat="server" ID="rptInstalledApplication">
                        <ItemTemplate>


                            <div class="card">
                                <div class="card-header" id='sph<%# Eval("WebId") %>'>
                                    <h2 class="mb-0">
                                        <button class="btn btn-link btn-block text-left" type="button" data-toggle="collapse" data-target="#sp<%# Eval("WebId") %>" aria-expanded="true" aria-controls="sp<%# Eval("WebId") %>">
                                            <%# Eval("WebUrl") %>
                                        </button>
                                    </h2>
                                </div>

                                <div id="sp<%# Eval("WebId") %>" class="collapse" aria-labelledby="sph<%# Eval("WebId") %>" data-parent="#accordionExample">
                                    <div class="card-body">
                                        <div class="container-fluid">
                                            <div class="row">
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">User Profile Type</label>
                                                    <div class="input-group mb-3">
                                                        <select class="form-control slcUPType" aria-label="Default select example">
                                                            <option value="ADFS" <%# Convert.ToString(Eval("UPAuthType")) == "ADFS" ? "selected" : "" %>>Trusted Provider</option>
                                                            <option value="Windows" <%# Convert.ToString(Eval("UPAuthType")) == "Windows" ? "selected" : "" %>>Windows</option>
                                                        </select>
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="User Profile Type" data-content="Web Arayüzüne ait erişim tipi. Eğer web sitesi erişimi için bir authentication provider belirlenmeyecekse veya Web Application Proxy kullanılacaksa WINDOWS seçilmeli aksi taktirde Trusted Provider seçeneği seçilmelidir.">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">Auth Type</label>
                                                    <div class="input-group mb-3">
                                                        <select class="form-control slcAuthType" aria-label="Default select example">
                                                            <option value="ADAL" <%# Convert.ToString(Eval("AuthType")) == "ADAL" ? "selected" : "" %>>ADAL</option>
                                                            <option value="MSAL" <%# Convert.ToString(Eval("AuthType")) == "MSAL" ? "selected" : "" %>>MSAL</option>
                                                        </select>
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="Auth Type" data-content="Mobil uygulamanın kıllandığı Auth library tipi">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <div class="row">
                                                        <div class="col">
                                                            <label for="basic-url" class="form-label">JWT Token LifeTime Check</label>
                                                            <div class="input-group mb-3">
                                                                <input class="form-control chkLifeTimeCheck" type="checkbox" value="" <%# Convert.ToBoolean(Eval("LifeTimeCheck")) ? "checked" : "" %>>
                                                            </div>
                                                        </div>
                                                        <div class="col">
                                                            <label for="basic-url" class="form-label">Debug Mode</label>
                                                            <div class="input-group mb-3">
                                                                <input class="form-control chkDebugMode" type="checkbox" value="" <%# Convert.ToBoolean(Eval("DebugMode")) ? "checked" : "" %>>
                                                            </div>
                                                        </div>
                                                        <div class="col">
                                                            <label for="basic-url" class="form-label">Active</label>
                                                            <div class="input-group mb-3">
                                                                <input class="form-control chkIsActive" type="checkbox" value="" <%# Convert.ToBoolean(Eval("IsActive")) ? "checked" : "" %>>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>




                                            <div class="row">
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">Audiance</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtAudiance" value='<%# Eval("Audiance") %>'>
                                                        <div class="input-group-append">

                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="Audiance" data-content="Oluşturulan JWT token içerisindeki 'aud' parametresi. Token validasyonu için kullanılmaktadır.">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">Issuer</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtIssuer" value='<%# Eval("Issuer") %>' />
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="Issuer" data-content="Oluşturulan JWT token içerisindeki 'iss' parametresi. Token validasyonu için kullanılmaktadır.">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">STS Discovery Endpoint</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtSTSDiscoveryUrl" value='<%# Eval("StsDiscoveryUrl") %>'>
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="STS Discovery Endpoint" data-content="JWT Tokeni oluşturan providera ait STS Discovery Endpoint adresi. Token validasyonu için kullanılmaktadır. ">?</a>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                            <hr />

                                            <div class="row">
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">SP Client Id</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtSPClientId" value='<%# Eval("SPClientId") %>'>
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="SP Client Id" data-content="SharePoint içerisine oluşturulan applicationa ait 'ClientID' bilgisi. ">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">SP Issuer Id</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtSPIssuerId" value='<%# Eval("SPIssuerId") %>' />
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="SP Issuer Id" data-content="'Trusted Security Token Issuer Oluşturma' adımında girilen Isser ID bilgisi. ">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">SPRealm</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtSPRealm" value='<%# Eval("SPRealm") %>' />
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="SPRealm" data-content="Mevcut SharePoint Farmına ait Realm bilgisi.">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>


                                            <div class="row">
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">Auth Provider Name</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtProviderName" value='<%# Eval("AuthProviderName") %>'>
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="Auth Provider Name" data-content="'trusted:LDAPCP' olarak girilmelidir. ">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">Certificate Path</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtCertificatePath" placeholder="\\certificate\cert.pbx" value='<%# Eval("CertificatePath") %>'>
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="Certificate Path" data-content="'Trusted Security Token Issuer Oluşturma' adımında kullanılan sertifikaya ait pbx dosyasının pathi">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col">
                                                    <label for="basic-url" class="form-label">Certificate Password</label>
                                                    <div class="input-group mb-3">
                                                        <input type="text" class="form-control txtCertificatePassword" value='<%# Eval("CertificatePassword") %>'>
                                                        <div class="input-group-append">
                                                            <a tabindex="0" class="btn btn-outline-secondary desc" role="button" data-toggle="popover" data-trigger="focus" title="Certificate Password" data-content="İlgili sertifikaya ait şifre">?</a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col">
                                                    <button type="button" class="btn btn-outline-primary saveBtn" data-webid='<%# Eval("WebId") %>'>Save</button>
                                                    <button type="button" class="btn btn-outline-danger removeBtn" data-webid='<%# Eval("WebId") %>'>Remove</button>
                                                </div>

                                                <%--<div class="col">
                                                    <button type="button" class="btn btn-primary importBtn" data-webid='<%# Eval("WebId") %>'>Import</button>
                                                    <button type="button" class="btn btn-danger exportBtn" data-webid='<%# Eval("WebId") %>'>Export</button>
                                                </div>--%>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
</asp:Content>
