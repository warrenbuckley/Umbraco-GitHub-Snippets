using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Octokit;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace GitHubSnippets.Controllers
{
    // Web API Calls can be access from this URL
    // /umbraco/snippets/*MethodName*
    [PluginController("Snippets")]
    public class GitHubController : UmbracoAuthorizedApiController
    {
        private static string _baseAPIUrl = "https://api.github.com";

        /// <summary>
        /// Get's the respository user from config file
        /// </summary>
        public string GetRepositoryUser()
        {
            //Returns a value from our settings config file
            return Settings.GetSetting("RepositoryUser");
        }

        /// <summary>
        /// Get's the respository name from config file
        /// </summary>
        /// <returns></returns>
        public string GetRepositoryName()
        {
            //Returns a value from our settings config file
            return Settings.GetSetting("RepositoryName");
        }

        /// <summary>
        /// Fetch contents of a specific file or folder from a GitHub Repo
        /// </summary>
        /// <param name="path">The path to the file or folder to request</param>
        public async Task<JToken> GetContent(string path)
        {
            //Get path from parameter
            //If the path length is greater than 1 AND it starts with a /
            if (path.Length > 1 && path.StartsWith("/"))
            {
                //Lets remove the starting /
                path = path.TrimStart('/');
            }

            //Get Settings from config file
            var repoUser    = Settings.GetSetting("RepositoryUser");
            var repo        = Settings.GetSetting("RepositoryName");

            //Format API Url to request
            var apiUrl = string.Format("{0}/repos/{1}/{2}/contents/{3}", _baseAPIUrl, repoUser, repo, path);

            //Do an async call to the GitHub API
            HttpClient client               = new HttpClient();
            HttpResponseMessage response    = await client.GetAsync(apiUrl);

            //If not success code throw a 404 not found
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound); 
            }

            //The remote JSON we recieve - gets it as a string
            //Need to convert it to a JSON object
            var content = await response.Content.ReadAsAsync<JToken>();

            //Return the JSON
            return content;
        }

        /// <summary>
        /// Fetch decoded contents of a specific file from a GitHub Repo
        /// </summary>
        /// <param name="path">The path to the file or folder to request</param>
        public async Task<string> GetContentDecoded(string path)
        {
            //Get path from parameter
            //If the path length is greater than 1 AND it starts with a /
            if (path.Length > 1 && path.StartsWith("/"))
            {
                //Lets remove the starting /
                path = path.TrimStart('/');
            }

            //Get Settings from config file
            var repoUser    = Settings.GetSetting("RepositoryUser");
            var repo        = Settings.GetSetting("RepositoryName");

            //Format API Url to request
            var apiUrl = string.Format("{0}/repos/{1}/{2}/contents/{3}", _baseAPIUrl, repoUser, repo, path);

            //Do an async call to the GitHub API
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            //If not success code throw a 404 not found
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            //The remote JSON we recieve - gets it as a string need to return a nice JSON object
            var content = await response.Content.ReadAsAsync<JToken>();

            //Decode the base64 content of the file using the helper
            var decodedContent = base64Decode(content["content"].ToString());

            return decodedContent;
        }

        /// <summary>
        /// http://forums.asp.net/t/645898.aspx
        /// </summary>
        /// <param name="data">The original base64 string to decode</param>
        /// <returns>Decoded string</returns>
        internal string base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder    = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode      = encoder.GetDecoder();

                byte[] todecode_byte    = Convert.FromBase64String(data);
                int charCount           = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char     = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);

                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }
    }
}
