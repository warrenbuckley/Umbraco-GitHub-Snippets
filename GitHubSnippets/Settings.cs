using System;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Xml;
using Umbraco.Core.Logging;

namespace GitHubSnippets
{
    public class Settings
    {
        public static XmlDocument SnippetsConfig
        {
            get
            {
                var us = (XmlDocument)HttpRuntime.Cache["SnippetsSettingsFile"];

                if (us == null)
                {
                    us = EnsureSettingsDocument();
                }
                    
                return us;
            }
        }

        private static XmlDocument EnsureSettingsDocument()
        {
            var settingsFile    = HttpRuntime.Cache["SnippetsSettingsFile"];
            var fullPath        = HostingEnvironment.MapPath(Config.ConfigPath);

            // Check for file in cache
            if (settingsFile == null)
            {
                var temp            = new XmlDocument();
                var settingsReader  = new XmlTextReader(fullPath);

                try
                {
                    temp.Load(settingsReader);
                    HttpRuntime.Cache.Insert("SnippetsSettingsFile", temp, new CacheDependency(fullPath));
                }
                catch (XmlException e)
                {
                    throw new XmlException("Your Snippets.config file fails to pass as valid XML. Refer to the InnerException for more information", e);
                }
                catch (Exception e)
                {
                    LogHelper.Error<Settings>("Error reading Snippets.config file", e);
                }

                settingsReader.Close();

                return temp;
            }
            else
            {
                return (XmlDocument)settingsFile;
            }
                
        }

        public static string GetSetting(string key)
        {
            XmlNode x = SnippetsConfig.SelectSingleNode(string.Format("//setting [@key = '{0}']", key));

            if (x != null)
            {
                return x.Attributes["value"].Value;
            }

            return string.Empty;
        }
    }
}