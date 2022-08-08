using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Juno.OnPrem.Auth.Common.Helpers
{
    [Guid("8F0D36AC-B492-4b7f-9B96-373BFB22B350")]
    public class ConfigCache : SPPersistedObject
    {     

        [Persisted]
        private string _installedWebApplication;
        public string InstalledWebApplication
        {
            get { return _installedWebApplication; }
            set { _installedWebApplication = value; }
        }   

        private const string PROPERTY_NAME = "SPChangeTokenConfig";
        public ConfigCache() { }
        public ConfigCache(SPPersistedObject parent) : base(PROPERTY_NAME, parent) { }

        protected override bool HasAdditionalUpdateAccess()
        {
            return false;
        }
        public static ConfigCache Settings
        {
            get
            {
                SPPersistedObject parent = SPFarm.Local;
                var obj = parent.GetChild<ConfigCache>(PROPERTY_NAME);
                if (obj == null)
                {
                    SPContext.Current.Web.AllowUnsafeUpdates = true;
                    obj = new ConfigCache(parent);
                    obj.Update(true);
                    SPContext.Current.Web.AllowUnsafeUpdates = false;
                }
                return obj;
            }
        }


    }
}
