using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Modules.Github
{
    public class GitHubRelease
    {
        public string tag_name { get; set; }
        public string published_at { get; set; }
        public string html_url { get; set; }
        public GitHubAsset[] assets { get; set; }
    }

    public class GitHubAsset
    {
        public string name { get; set; }
        public string browser_download_url { get; set; }
    }


    public class GitHubApi
    {
        public static async Task<GitHubRelease> GetLatestReleaseAsync(string repo)
        {
            string apiUrl = $"https://api.github.com/repos/zGiuly/{repo}/releases/latest";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("C# App");
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GitHubRelease>(responseBody);
            }
        }

        public static string GetAssetDownloadUrl(GitHubRelease release, string assetName)
        {
            var asset = release.assets.FirstOrDefault(a => a.name == assetName);
            return asset?.browser_download_url;
        }
    }
}
