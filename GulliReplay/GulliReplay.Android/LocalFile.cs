using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using GulliReplay.Droid;

[assembly: Dependency (typeof (LocalFile))]
namespace GulliReplay.Droid
{
    public class LocalFile : ILocalFile
    {
        private string _root = "";
        public string Root
        {
            get
            {
                if (_root == "")
                    _root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                return _root;
            }
        }

        public void Save(string FileName, Image image)
        {
            throw new NotImplementedException();
        }

        public void Save(string FileName, string str)
        {
            var documentsPath = Root;
            var filePath = Path.Combine(documentsPath, FileName);
            File.WriteAllText(filePath, str);
        }

        public void Save(string FileName, object obj)
        {
            if (obj is string)
            {
                Save(FileName, (string)obj);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public Image LoadImage(string FileName)
        {
            throw new NotImplementedException();
        }

        public string LoadString(string FileName)
        {
            var documentsPath = Root;
            var filePath = Path.Combine(documentsPath, FileName);
            return File.ReadAllText(filePath);
        }

        public T Load<T>(string FileName)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)LoadString(FileName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}