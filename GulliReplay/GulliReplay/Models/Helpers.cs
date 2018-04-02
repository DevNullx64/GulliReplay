using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GulliReplay
{
    public static class Helpers
    {
        public static void SortedAdd<T>(this ObservableCollection<T> collection, T item) where T : IComparable<T>
        {
            if ((collection != null) && (item != null))
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    int cmp = collection[i].CompareTo(item);
                    if (cmp > 0)
                        collection.Insert(i, item);
                    if (cmp >= 0)
                        return;
                }
                collection.Add(item);
            }
        }
        public static void SortedAdd<T>(this ObservableCollection<T> collection, IEnumerable<T> items) where T : IComparable<T>
        {
            if ((collection != null) && (items != null))
            {
                foreach (T item in items)
                    SortedAdd(collection, item);
            }
        }

        public static WebResponse GetResponseSync(this Uri uri) => (HttpWebRequest.Create(uri)).GetResponseSync();
        public static WebResponse GetResponseSync(this WebRequest request)
        {
            Task<WebResponse> task = request.GetResponseAsync();
            task.Wait();
            return task.Result;
        }

        public static string GetContent(this Uri uri) => HttpWebRequest.Create(uri).GetContent();
        public static string GetContent(this WebRequest request)
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