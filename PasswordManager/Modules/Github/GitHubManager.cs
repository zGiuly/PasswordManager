using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager.Modules.Github
{
    public class GitHubManager
    {
        public static readonly string softwareVersion = "1.1";
        public static readonly string AssetName = "PasswordManager.exe";

        private string repository;

        private GitHubRelease release;

        public GitHubManager(string repository)
        {
            this.repository = repository;
            Inizialize();
        }

        private async void Inizialize()
        {
            try
            {
                release = await GitHubApi.GetLatestReleaseAsync(repository);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task<Boolean> CheckUpdate()
        {
            if (release == null) return false;

            if(release.tag_name.ToLower() == softwareVersion.ToLower())
            {
                return false;
            }
            return true;
        }

        public async Task<Boolean> DownloadLastVersion()
        {
            var status = await CheckUpdate();

            if (!status) return false;

            string url = GitHubApi.GetAssetDownloadUrl(release, AssetName);

            if(url == null) return false;

            string existingExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AssetName);
            string backupExePath = existingExePath + ".old";

            if (File.Exists(existingExePath))
            {
                File.Move(existingExePath, backupExePath);
            }

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                File.WriteAllBytes(existingExePath, fileBytes);

                if (File.Exists(backupExePath))
                {
                    File.Delete(backupExePath);
                }
            }
            return true;
        }
    }
}
