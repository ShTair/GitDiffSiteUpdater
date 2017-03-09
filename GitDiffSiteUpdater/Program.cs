using GitDiffSiteUpdater.Models;
using System.Threading.Tasks;

namespace GitDiffSiteUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(args[0]).Wait();
        }

        private static async Task Run(string settingsPath)
        {
            var settings = await SiteSettings.LoadAsync(settingsPath);
            if (settings == null)
            {
                settings = new SiteSettings();
                await SiteSettings.SaveAsync(settingsPath, settings);
                return;
            }
        }
    }
}
