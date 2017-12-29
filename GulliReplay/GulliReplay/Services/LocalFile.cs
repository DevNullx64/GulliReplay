using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace GulliReplay
{
    public interface ILocalFile
    {
        string Root { get; }
        void Save(string FileName, object obj);
        T Load<T>(string FileName);
    }

    class LocalFile
    { 
        private static ILocalFile _interface = DependencyService.Get<ILocalFile>();
        public static string Root { get => _interface.Root; }
        public static T Load<T>(string FileName) => _interface.Load<T>(FileName);
        public static void Save(string FileName, object obj) => _interface.Save(FileName, obj);
    }
}
