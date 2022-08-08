using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Juno.OnPrem.Auth.Common.Constants
{
    public enum AuthType
    {
        Windows,
        ADFS
    }
    public class Constants
    {
        //public static AuthType AuthType
        //{
        //    get
        //    {
        //        var auth = WebConfigurationManager.AppSettings.Get("SPAuthType");
        //        return (AuthType)Enum.Parse(typeof(AuthType), auth);

        //    }
        //}
        public class Headers
        {
            public static readonly string ChangeTokenHeaderKey = "ChangeToken";
            public static readonly string AuthorizationHeaderKey = "Authorization";
        }

        public class ADFS
        {
            //public static readonly string Domain = WebConfigurationManager.AppSettings.Get("AdfsDomain");
            //public static readonly string Audiance = WebConfigurationManager.AppSettings.Get("AdfsServerRoleIdentifier");
            //public static readonly string Issuer = "{0}/adfs/services/trust";
            //public static readonly string DiscoveryEndpointUrl = "{0}/adfs/.well-known/openid-configuration";
        }

        public static readonly string UpnKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
        public static readonly string SPRestApiPrefix = "/_api/";
        public static readonly string SPVtiBinPrefix = "/_vti_bin/";
        public static readonly string IISModuleName = "SPChangeToken";
        public static readonly string IISModuleTypeName = "Juno.OnPrem.Auth.SPTokenChangeModule.Modules.SPTokenChange, Juno.OnPrem.Auth.SPTokenChangeModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1f7f9262412316b5";
        public static readonly string WebConfigWebId = "WebId";
        public static readonly string CurrentWebId = WebConfigurationManager.AppSettings.Get("WebId");
    }
}
