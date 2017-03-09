using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace GitDiffSiteUpdater.Models
{
    class SiteSettings
    {
        public string Base { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string GitPath { get; set; }

        public string ReposPath { get; set; }

        public string Before { get; set; }

        public string After { get; set; }

        public static async Task<SiteSettings> LoadAsync(string path)
        {
            try
            {
                using (var reader = File.OpenText(path))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<SiteSettings>(json);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task SaveAsync(string path, SiteSettings value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            using (var writer = File.CreateText(path))
            {
                await writer.WriteAsync(json);
            }
        }
    }
}
