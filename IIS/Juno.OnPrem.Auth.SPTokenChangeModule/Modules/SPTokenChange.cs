using Juno.OnPrem.Auth.Common.Constants;
using Juno.OnPrem.Auth.Common.Entities;
using Juno.OnPrem.Auth.Common.Helpers;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Juno.OnPrem.Auth.SPTokenChangeModule.Modules
{
    public class SPTokenChange : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.Application_BeginRequest);
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            HttpApplication app = ((HttpApplication)source);
            var currentUrl = app.Request.Url.AbsoluteUri;

            if (currentUrl.ToLower().Contains(Constants.SPRestApiPrefix.ToLower()) || currentUrl.ToLower().Contains(Constants.SPVtiBinPrefix.ToLower()))
            {
                Logger.WriteLog(Logger.Category.Medium, "IISMODULE", "Init IISModule");
                var host = currentUrl.Split(new string[] { Constants.SPRestApiPrefix }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (app.Request.Headers.AllKeys.Contains(Constants.Headers.ChangeTokenHeaderKey, StringComparer.OrdinalIgnoreCase))
                {
                    try
                    {
                        Logger.WriteLog(Logger.Category.Medium, "IISMODULE", $"Entering ChangeToken scope. | WebId: {Constants.CurrentWebId}");
                        var currentToken = app.Request.Headers[Constants.Headers.AuthorizationHeaderKey];
                        ConfigCacheItem config = ConfigHelper.GetCurrentConfigItem();
                        if (config.IsActive)
                        {
                            var validateResult = Helpers.ValidateToken(currentToken.Replace("Bearer ", ""));
                            Logger.WriteLog(Logger.Category.Medium, "IISMODULE", $"Token validation status : {validateResult.Status} | WebId: {Constants.CurrentWebId}");
                            if (validateResult.Status)
                            {
                                var upn = validateResult.User.Claims.Where(x => x.Type == Constants.UpnKey).Select(x => x.Value).FirstOrDefault();
                                Logger.WriteLog(Logger.Category.Medium, "IISMODULE", $"Creating access token for user  : {upn} | WebId: {Constants.CurrentWebId}");
                                var accessToken = string.Empty;
                                AuthType authType = (AuthType)Enum.Parse(typeof(AuthType), config.UPAuthType);
                                switch (authType)
                                {
                                    case AuthType.Windows:
                                        accessToken = TokenHelper.GetS2SAccessTokenWithWindowsIdentity(new Uri(host), new WindowsIdentity(upn));
                                        break;
                                    case AuthType.ADFS:
                                        accessToken = TokenHelper.GetS2SAccessTokenWithADFSIdentity(new Uri(host), upn);
                                        break;
                                    default:
                                        accessToken = TokenHelper.GetS2SAccessTokenWithWindowsIdentity(new Uri(host), new WindowsIdentity(upn));
                                        break;
                                }

                                Logger.WriteLog(Logger.Category.Information, "IISMODULE", $"Created SP Access Token for user  : {upn} | Token : {accessToken} | WebId: {Constants.CurrentWebId}");
                                app.Request.Headers[Constants.Headers.AuthorizationHeaderKey] = "Bearer " + accessToken;
                            }
                            else
                            {
                                Logger.WriteLog(Logger.Category.Unexpected, "IISMODULE", $"Token validation error  : {validateResult.Message} | WebId: {Constants.CurrentWebId}");
                                // throw new Exception(validateResult.Message);
                                throw new HttpException(401, validateResult.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(Logger.Category.Unexpected, "IISMODULE", $"Unexpected error  : {ex.Message} | WebId: {Constants.CurrentWebId}");
                        throw new HttpException(500, ex.Message);
                    }
                }
            }

        }
    }
}
