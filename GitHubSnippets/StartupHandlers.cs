using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umbraco.Core;
using umbraco.presentation.masterpages;
using umbraco.uicontrols;

namespace GitHubSnippets
{
    public class StartupHandlers : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //throw new NotImplementedException();
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //throw new NotImplementedException();
        }

        
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //When Umbraco Has Started Up...
            //Hook into the UmbracoPage Load event - which is any Umbraco backoffice page
            umbracoPage.Load += umbracoPage_Load;
        }


        void umbracoPage_Load(object sender, EventArgs e)
        {
            //Cast sender as an Umbraco Page object
            var pageReference = (umbracoPage)sender;

            //Get the path of the current page
            var path = pageReference.Page.Request.Path.ToLower();

            //Check if the path of the page ends in either of the following...
            if (path.EndsWith("settings/views/editview.aspx") == true || path.EndsWith("settings/edittemplate.aspx"))
            {
                //Try & get body panel control from the page (as still .NET page)
                var c2 = GetPanel1Control(pageReference);

                //If we have found it then...
                if (c2 != null)
                {
                    //Cast the control we found as an Umbraco Panel object
                    var panel = (UmbracoPanel)c2;

                    //This is the Javascript we want to run when our button is clicked
                    //Including what happens when the dialog closes & invokes our callback
                    var javascript = @"UmbClientMgr.openAngularModalWindow({
                                        template: '/app_plugins/snippets/snippet-dialog.html',
                                        callback: function(data) {
                                            var snippet = JSON.parse(data.code);
                                            UmbClientMgr.contentFrame().UmbEditor.Insert(snippet, '');
                                            top.UmbSpeechBubble.ShowMessage('success', 'Snippet Inserted', 'Yipee you have sucessfully inserted a snippet from GitHub called: ' + data.name);
                                        }
                                    });";
                    
                    //Lets create a new button and add it to the panel control
                    var snippetBtn              = panel.Menu.NewButton(-1);
                    snippetBtn.Text             = "Insert Snippet";
                    snippetBtn.ToolTip          = "Insert Snippet";
                    snippetBtn.ButtonType       = MenuButtonType.Primary;
                    snippetBtn.Icon             = "code";
                    snippetBtn.OnClientClick    = javascript;

                }
            }

        }


        //Get the body panel control
        private Control GetPanel1Control(umbracoPage up)
        {
            var cph = (ContentPlaceHolder)up.FindControl("body");

            return cph.FindControl("body_Panel1_container");
        }
    }
}
