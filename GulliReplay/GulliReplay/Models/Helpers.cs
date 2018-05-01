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
        public static WebResponse GetResponseSync(this WebRequest request) => request.GetResponseAsync().GetAwaiter().GetResult();

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

        private static object BackgroundDownloaderLocker = new object();
        private static HashSet<string> BackgroundDownloading = new HashSet<string>();
        public static void BackgroundDownloader(Uri uri, string fileName, Action<object> action = null, object actionState = null)
        {
            if (!fileName.StartsWith(LocalFile.Root))
                fileName = Path.Combine(LocalFile.Root, fileName);

            bool downloading;
            lock (BackgroundDownloaderLocker)
            {
                downloading = BackgroundDownloading.Contains(fileName);
                if (!downloading)
                    BackgroundDownloading.Add(fileName);
            }

            if (!downloading)
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.BeginGetResponse((o) =>
                    {
                        try
                        {
                            Tuple<HttpWebRequest, string> state = (Tuple<HttpWebRequest, string>)o.AsyncState;
                            HttpWebResponse response = (HttpWebResponse)state.Item1.EndGetResponse(o);
                            using (Stream content = response.GetResponseStream())
                            using (Stream file = File.Create(state.Item2))
                                content.CopyTo(file);

                            lock (BackgroundDownloaderLocker)
                            {
                                BackgroundDownloading.Remove(state.Item2);
                            }
                            action?.Invoke(actionState);
                        }
                        catch (Exception e)
                        {
                            Debug.Write(e.Message);
                        }
                    }, new Tuple<HttpWebRequest, string>(request, fileName));
                }
                catch (Exception e)
                {
                    Debug.Write(e.Message);
                }
        }
    }
}