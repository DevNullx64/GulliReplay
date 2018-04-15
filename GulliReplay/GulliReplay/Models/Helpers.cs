using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace GulliReplay
{
    public static class Helpers
    {
        public static WebResponse GetResponseSync(this Uri uri) => (HttpWebRequest.Create(uri)).GetResponseSync();
        public static WebResponse GetResponseSync(this WebRequest request)
        {
            return request.GetResponseAsync().GetAwaiter().GetResult();
        }

        public static string GetContent(this Uri uri) => GetContent(HttpWebRequest.Create(uri) as HttpWebRequest);
        public static string GetContent(this HttpWebRequest request)
        {
            try
            {
                if (request == null)
                    throw new NullReferenceException("GetContent: 'request' is null.");
                else
                {
                    using (WebResponse response = request.GetResponseSync())
                        if (response == null)
                            throw new NullReferenceException("GetContent: 'request' is null.");
                        else
                            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                                return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw e;
            }
        }

        public static bool Download(Uri uri, string fileName)
        {
            if (!fileName.StartsWith(LocalFile.Root))
                fileName = Path.Combine(LocalFile.Root, fileName);

            try
            {
                WebRequest request = HttpWebRequest.Create(uri);
                using (WebResponse response = request.GetResponseSync())
                using (Stream content = response.GetResponseStream())
                using (Stream file = File.Create(fileName))
                    content.CopyTo(file);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return false;
            }
            return true;
        }
    }
}