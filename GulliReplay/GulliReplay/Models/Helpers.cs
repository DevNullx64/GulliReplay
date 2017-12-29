using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace GulliReplay
{
    public static class Helpers
    {
        public const string updateString = " (en cours de mise à jour...)";

        private static AutoResetEvent GetResponseSyncEvent = new AutoResetEvent(false);
        private class GetResponseSyncObject
        {
            public WebRequest Request;
            public WebResponse Response;

            public GetResponseSyncObject(WebRequest request, WebResponse response = null)
            {
                Request = request;
                Response = response;
            }
        }

        public static WebResponse GetResponseSync(this Uri uri)
        {
            return (HttpWebRequest.Create(uri)).GetResponseSync();
        }
        public static WebResponse GetResponseSync(this WebRequest request)
        {
            GetResponseSyncObject data = new GetResponseSyncObject(request);
            request.BeginGetResponse(new AsyncCallback(GetResponseSyncCallBack), data);
            GetResponseSyncEvent.WaitOne();
            return data.Response;
        }
        private static void GetResponseSyncCallBack(IAsyncResult asynchronousResult)
        {
            GetResponseSyncObject data = (GetResponseSyncObject)asynchronousResult.AsyncState;
            data.Response = data.Request.EndGetResponse(asynchronousResult);
            GetResponseSyncEvent.Set();
        }

        public static string GetContent(this Uri uri)
        {
            return HttpWebRequest.Create(uri).GetContent();
        }
        public static string GetContent(this WebRequest request)
        {
            using (WebResponse response = request.GetResponseSync())
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                return sr.ReadToEnd();
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
            } catch(Exception e)
            {
                Debug.Write(e.Message);
                return false;
            }
            return true;
        }
    }
}
