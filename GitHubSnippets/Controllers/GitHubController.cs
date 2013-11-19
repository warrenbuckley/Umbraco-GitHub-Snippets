using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Octokit;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace GitHubSnippets.Controllers
{
    [PluginController("Snippets")]
    public class GitHubController : UmbracoAuthorizedApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRepositoryUser()
        {
            return Settings.GetSetting("RepositoryUser");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRepositoryName()
        {
            return Settings.GetSetting("RepositoryName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<ContentsResponse> GetContent(string path)
        {
            //Get path from parameter
            if (path.StartsWith("/"))
            {
                path = path.TrimStart('/');
            }

            //Get Settings from config file
            var repoUser    = Settings.GetSetting("RepositoryUser");
            var repo        = Settings.GetSetting("RepositoryName");

            var github  = new GitHubClient(new ProductHeaderValue("UmbracoGitHubSnippets"));
            var content = await github.Repository.Content.GetContents(repoUser, repo, path);

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string> GetContentDecoded(string path)
        {
            //Get path from parameter
            if (path.StartsWith("/"))
            {
                path = path.TrimStart('/');
            }

            //Get Settings from config file
            var repoUser    = Settings.GetSetting("RepositoryUser");
            var repo        = Settings.GetSetting("RepositoryName");

            var github  = new GitHubClient(new ProductHeaderValue("UmbracoGitHubSnippets"));
            var content = await github.Repository.Content.GetContents(repoUser, repo, path);

            //Decode the base64 content of the file
            var decodedContent = base64Decode(content.Content);

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