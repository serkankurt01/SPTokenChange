using Juno.OnPrem.Auth.Common.Entities;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juno.OnPrem.Auth.Common.Helpers
{
    public class ConfigHelper
    {
        public static List<ConfigCacheItem> GetConfigItems()
        {
            var persistedSettings = ConfigCache.Settings;
            if (persistedSettings.InstalledWebApplication != null)
                return Helpers.StringToObject(persistedSettings.InstalledWebApplication) as List<ConfigCacheItem>;
            else
                return new List<ConfigCacheItem>();
        }

        public static ConfigCacheItem GetConfigItem(string webId)
        {
            ///TODO : Cachleme yapılacak.
            List<ConfigCacheItem> items = GetConfigItems();
            return items.Where(x => x.WebId.ToString() == webId).FirstOrDefault();
        }
        public static ConfigCacheItem GetCurrentConfigItem()
        {
            ///TODO : Cachleme yapılacak.
            List<ConfigCacheItem> items = GetConfigItems();
            return items.Where(x => x.WebId.ToString() == Constants.Constants.CurrentWebId).FirstOrDefault();
        }

        public static List<ConfigCacheItem> SaveConfigItems(List<ConfigCacheItem> items)
        {
            Logger.WriteLog(Logger.Category.Medium, "ADMIN", "Saving web.config params.");
            var persistedSettings = ConfigCache.Settings;
            persistedSettings.InstalledWebApplication = Helpers.ObjectToString(items);
            persistedSettings.Update();
            return items;
        }
        public static List<ConfigCacheItem> RemoveConfigItems(string webId)
        {
            List<ConfigCacheItem> items = GetConfigItems();
            var checkItem = items.Where(x => x.WebId.ToString() == webId).FirstOrDefault();
            if (checkItem != null)
                items.Remove(checkItem);
            return SaveConfigItems(items);
        }

    }
}
