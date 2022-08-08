using Juno.OnPrem.Auth.Common.Entities;
using Juno.OnPrem.Auth.Common.Constants;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Juno.OnPrem.Auth.Common.Helpers
{
    public class Helpers
    {

        public static void AddToWebConfig(SPWebApplication webApp)
        {
            /// TODO : Düzenlenecek 

            RemoveWebConfig(webApp);
            AddAppSettingsConfigs(webApp);
            AddModuleConfig(webApp);
            webApp.Update();
            // Applies the list of web.config modifications to all Web applications in this Web service across the farm.
            webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
        }


        public static void AddModuleConfig(SPWebApplication webApp)
        {
            var WebConfigModification = new SPWebConfigModification();
            WebConfigModification.Path = "configuration/system.webServer/modules";
            WebConfigModification.Name = string.Format("add[@name='{0}'][@type='{1}']", Constants.Constants.IISModuleName, Constants.Constants.IISModuleTypeName);
            WebConfigModification.Value = string.Format("<add name=\"{0}\" type=\"{1}\" />", Constants.Constants.IISModuleName, Constants.Constants.IISModuleTypeName);
            WebConfigModification.Sequence = 0;
            WebConfigModification.Type = 0;
            WebConfigModification.Owner = "SPTokenChangeModule";
            webApp.WebConfigModifications.Add(WebConfigModification);

        }

        public static void AddAppSettingsConfigs(SPWebApplication webApp)
        {
            var WebConfigModification = new SPWebConfigModification();
            WebConfigModification.Path = "configuration/appSettings";
            WebConfigModification.Name = string.Format("add[@key='{0}'][@value='{1}']", Constants.Constants.WebConfigWebId, webApp.Id.ToString());
            WebConfigModification.Value = string.Format("<add key=\"{0}\" value=\"{1}\" />", Constants.Constants.WebConfigWebId, webApp.Id.ToString());
            WebConfigModification.Sequence = 0;
            WebConfigModification.Type = 0;
            WebConfigModification.Owner = "SPTokenChangeAppSettings";
            webApp.WebConfigModifications.Add(WebConfigModification);

        }


        public static void RemoveWebConfig(SPWebApplication webApp)
        {

            var ogoo = webApp.WebConfigModifications.Where(x => x.Owner == "OGOO").FirstOrDefault();
            if (ogoo != null)
                webApp.WebConfigModifications.Remove(ogoo);


            var SPTokenChangeModule = webApp.WebConfigModifications.Where(x => x.Owner == "SPTokenChangeModule").FirstOrDefault();
            if (SPTokenChangeModule != null)
                webApp.WebConfigModifications.Remove(SPTokenChangeModule);
            var SPTokenChangeAppSettings = webApp.WebConfigModifications.Where(x => x.Owner == "SPTokenChangeAppSettings").FirstOrDefault();
            if (SPTokenChangeAppSettings != null)
                webApp.WebConfigModifications.Remove(SPTokenChangeAppSettings);
            webApp.Update();
            webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
        }

        public static string ObjectToString(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static object StringToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }
        }

        public static TokenValidateResult ValidateToken(string token)
        {
            ConfigCacheItem settings = ConfigHelper.GetCurrentConfigItem();
            TokenValidateResult result = new TokenValidateResult();
            try
            {
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(settings.StsDiscoveryUrl, new OpenIdConnectConfigurationRetriever());
                OpenIdConnectConfiguration openIdConfig = AsyncHelper.RunSync(async () => await configurationManager.GetConfigurationAsync(CancellationToken.None));
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidIssuer = settings.Issuer,
                    ValidIssuers = new[] { settings.Issuer },
                    ValidAudiences = new[] { settings.Audiance },
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    ValidateLifetime = settings.LifeTimeCheck
                };

                var validatedToken = (Microsoft.IdentityModel.Tokens.SecurityToken)new JwtSecurityToken();
                var user = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                result.Status = true;
                result.User = user;
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.User = null;
                result.Message = ex.Message;
                return result;
            }
        }
    }
}
