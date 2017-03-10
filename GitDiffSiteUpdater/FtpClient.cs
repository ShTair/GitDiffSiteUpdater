using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GitDiffSiteUpdater
{
    class FtpClient
    {
        private NetworkCredential _credential;
        private HashSet<string> _ds;
        private Uri _base;

        public FtpClient(string userName, string password, Uri baseUri)
        {
            _credential = new NetworkCredential(userName, password);
            _ds = new HashSet<string>();
            _base = baseUri;
        }

        public async Task<List<string>> ListDirectory(Uri uri)
        {
            var u = new Uri(_base, uri);

            using (var res = await GetResponseAsync(u, WebRequestMethods.Ftp.ListDirectory))
            using (var stream = res.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.Default))
            {
                string line;
                var list = new List<string>();
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line == ".") continue;
                    if (line == "..") continue;
                    var p = new Uri(u, line).LocalPath;
                    list.Add(p);
                    _ds.Add(p);
                }

                return list;
            }
        }

        public async Task MakeDirectory(Uri uri)
        {
            using (var res = await GetResponseAsync(uri, WebRequestMethods.Ftp.MakeDirectory))
            {
            }
        }

        public async Task Upload(Uri uri, Stream stream)
        {
            var u = new Uri(_base, uri);
            var d = new Uri(u, ".");
            await CreateDirectory(d, false);

            using (var res = await GetResponseAsync(u, WebRequestMethods.Ftp.UploadFile, stream))
            {
            }
        }

        private async Task CreateDirectory(Uri dir, bool l)
        {
            var lp = dir.LocalPath;
            if (lp == "/")
            {
                await ListDirectory(dir);
                return;
            }

            if (_ds.Contains(lp.Remove(lp.Length - 1)))
            {
                if (l) await ListDirectory(dir);
                return;
            }

            var bd = new Uri(dir, "..");
            await CreateDirectory(bd, true);

            if (_ds.Contains(lp.Remove(lp.Length - 1)))
            {
                if (l) await ListDirectory(dir);
                return;
            }

            await MakeDirectory(dir);
            _ds.Add(lp.Remove(lp.Length - 1));
        }

        private async Task<FtpWebResponse> GetResponseAsync(Uri uri, string method)
        {
            var req = (FtpWebRequest)WebRequest.Create(uri);
            req.Credentials = _credential;
            req.Method = method;
            req.KeepAlive = false;
            req.UsePassive = true;
            req.EnableSsl = true;

            return (FtpWebResponse)await req.GetResponseAsync();
        }

        private async Task<FtpWebResponse> GetResponseAsync(Uri uri, string method, Stream stream)
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
