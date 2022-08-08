using Juno.OnPrem.Auth.Common.Helpers;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using System;
using System.Linq;
using System.Collections.Generic;
using Juno.OnPrem.Auth.Common.Entities;

namespace Juno.OnPrem.Auth.SPTokenChange.Layouts.Juno.OnPrem.Auth.SPTokenChange
{
    public partial class Setup : LayoutsPageBase
    {
        private SPWebApplication webApplication;
        private List<string> installedWebAppication;
        private List<ConfigCacheItem> items = ConfigHelper.GetConfigItems();
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                Logger.WriteLog(Logger.Category.Medium, "ADMIN", "Entering ChangeToken admin scope.");
                if (!Page.IsPostBack)
                {
                    rptInstalledApplication.DataSource = items;
                    rptInstalledApplication.DataBind();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, "ADMIN", ex.Message);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);          
        }

        protected void WebAppSelector_OnContextChange(object sender, EventArgs e)
        {
            Microsoft.SharePoint.Administration.SPWebApplication currentWebApp = WebAppSelector.CurrentItem;
            if (currentWebApp != null)
            {
                webApplication = currentWebApp;
            }
        }


        public void btnUpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.WriteLog(Logger.Category.Medium, "ADMIN", "Entering btnUpdateButton_Click scope.");
                var checkItem = items.Where(x => x.WebId == WebAppSelector.CurrentItem.Id).FirstOrDefault();
                if (checkItem == null)
                    items.Add(new ConfigCacheItem()
                    {
                        WebUrl = WebAppSelector.CurrentItem.Name,
                        WebId = WebAppSelector.CurrentItem.Id
                    });
                ConfigHelper.SaveConfigItems(items);
                Helpers.AddToWebConfig(WebAppSelector.CurrentItem);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);

            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.Category.Unexpected, "ADMIN", ex.Message);
            }
        }        
    }
}
