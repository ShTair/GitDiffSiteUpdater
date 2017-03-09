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
            using (var res = await GetResponseAsync(uri, WebRequestMethods.Ftp.ListDirectory))
            using (var stream = res.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.Default))
            {
                var data = await reader.ReadToEndAsync();
                Console.WriteLine(data);
                return data;
            }
        }

        public async Task Ex(string uri)
        {
            using (var res = await GetResponseAsync(uri, WebRequestMethods.Ftp.MakeDirectory))
            {
            }
        }

        public async Task Upload(string uri, Stream stream)
        {
            using (var res = await GetResponseAsync(uri, WebRequestMethods.Ftp.UploadFile, stream))
            {
            }
        }

        private async Task<FtpWebResponse> GetResponseAsync(string uri, string method)
        {
            var req = (FtpWebRequest)WebRequest.Create(uri);
            req.Credentials = _credential;
            req.Method = method;
            req.KeepAlive = false;
            req.UsePassive = true;
            req.EnableSsl = true;

            return (FtpWebResponse)await req.GetResponseAsync();
        }

        private async Task<FtpWebResponse> GetResponseAsync(string uri, string method, Stream stream)
        {
            var req = (FtpWebRequest)WebRequest.Create(uri);
            req.Credentials = _credential;
            req.Method = method;
            req.KeepAlive = false;
            req.UsePassive = true;
            req.EnableSsl = true;
            using (var dst = await req.GetRequestStreamAsync())
            {
                await stream.CopyToAsync(dst);
            }

            return (FtpWebResponse)await req.GetResponseAsync();
        }
    }
}
