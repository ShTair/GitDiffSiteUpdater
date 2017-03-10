using GitDiffSiteUpdater.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            //await SiteSettings.SaveAsync(settingsPath, settings);
            var client = new FtpClient(settings.UserName, settings.Password);

            var pi = new ProcessStartInfo
            {
                FileName = settings.GitPath,
                WorkingDirectory = settings.ReposPath,
                Arguments = $"diff --name-only {settings.Before}..{settings.After}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var _ds = new HashSet<string>();

            using (var p = Process.Start(pi))
            {
                string ap;
                while ((ap = await p.StandardOutput.ReadLineAsync()) != null)
                {
                    Console.WriteLine(ap);
                    var a = Path.Combine(settings.ReposPath, ap.Replace('/', '\\'));
                    if (File.Exists(a))
                    {
                        var uri = settings.Base + ap;
                        var d = settings.Base + Path.GetDirectoryName(ap);

                        if (!_ds.Contains(d))
                        {
                            _ds.Add(d);
                            try
                            {
                                await client.Ex(d);
                            }
                            catch { }
                        }

                        using (var stream = File.OpenRead(a))
                        {
                            await client.Upload(uri, stream);
                        }
                    }
                }
            }
        }
    }
}
