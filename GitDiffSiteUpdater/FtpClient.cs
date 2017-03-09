using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GitDiffSiteUpdater
{
    class FtpClient
    {
        private NetworkCredential _credential;

        public FtpClient(string userName, string password)
        {
            _credential = new NetworkCredential(userName, password);
        }

        public async Task<string> ListDirectory(string uri)
        {
            using (var res = await GetResponseAsync(uri))
            using (var stream = res.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.Default))
            {
                var data = await reader.ReadToEndAsync();
                Console.WriteLine(data);
                return data;
            }
        }

        private async Task<FtpWebResponse> GetResponseAsync(string uri)
        {
            var req = (FtpWebRequest)WebRequest.Create(uri);
            req.Credentials = _credential;
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            req.KeepAlive = false;
            req.UsePassive = true;
            req.EnableSsl = true;

            return (FtpWebResponse)await req.GetResponseAsync();
        }
    }
}
