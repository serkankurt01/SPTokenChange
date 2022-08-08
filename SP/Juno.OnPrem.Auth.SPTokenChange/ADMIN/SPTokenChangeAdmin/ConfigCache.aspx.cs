using Juno.OnPrem.Auth.Common.Entities;
using Juno.OnPrem.Auth.Common.Helpers;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;

namespace Juno.OnPrem.Auth.SPTokenChange.Layouts.Juno.OnPrem.Auth.SPTokenChange
{
    public partial class ConfigCache : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string param = this.Page.Request.QueryString["ops"];

            switch (param)
            {
                case "Remove":
                    var webId = this.Page.Request.QueryString["webId"];
                    SPContext.Current.Web.AllowUnsafeUpdates = true;
                    var webApp = SPFarm.Local.Services.GetValue<SPWebService>().WebApplications.Where(x => x.Id.ToString() == webId).FirstOrDefault();
                    Helpers.RemoveWebConfig(webApp);
                    ConfigHelper.RemoveConfigItems(webId);
                    SPContext.Current.Web.AllowUnsafeUpdates = false;
                    break;
                case "Save":
                    break;
                default:
                    break;
            }
        }
        [WebMethod]
        [ScriptMethod]
        public static void SaveConfig(ConfigCacheItem item)
        {
            List<ConfigCacheItem> items = ConfigHelper.GetConfigItems();
            var checkItem = items.Where(x => x.WebId == item.WebId).FirstOrDefault();
            if (checkItem != null)
            {
                /// TODO : Düzenlenecek
                checkItem.Audiance = item.Audiance;
                checkItem.AuthType = item.AuthType;
                checkItem.CertificatePassword = item.CertificatePassword;
                checkItem.CertificatePath = item.CertificatePath;
                checkItem.Issuer = item.Issuer;
                checkItem.LifeTimeCheck = item.LifeTimeCheck;
                checkItem.SPClientId = item.SPClientId;
                checkItem.SPIssuerId = item.SPIssuerId;
                checkItem.SPRealm = item.SPRealm;
                checkItem.StsDiscoveryUrl = item.StsDiscoveryUrl;
                checkItem.UPAuthType = item.UPAuthType;
                checkItem.AuthProviderName = item.AuthProviderName;
                checkItem.DebugMode = item.DebugMode;
                checkItem.IsActive = item.IsActive;
            }                
            ConfigHelper.SaveConfigItems(items);
        }

        [WebMethod]
        [ScriptMethod]
        public static void ExportConfig(ConfigCacheItem item)
        {
            List<ConfigCacheItem> items = ConfigHelper.GetConfigItems();
            var checkItem = items.Where(x => x.WebId == item.WebId).FirstOrDefault();
            if (checkItem != null)
            {
                
            }
           
        }

        [WebMethod]
        [ScriptMethod]
        public static void ImportConfig(ConfigCacheItem item)
        {
            List<ConfigCacheItem> items = ConfigHelper.GetConfigItems();
            var checkItem = items.Where(x => x.WebId == item.WebId).FirstOrDefault();
            if (checkItem != null)
            {

            }
        }
    }
}
