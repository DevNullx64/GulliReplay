using System;
using System.Threading.Tasks;
using Windows.Storage;
using Xamarin.Forms;
using GulliReplay;
using GulliReplay.UWP;

[assembly: Dependency(typeof(LocalFile))]
namespace GulliReplay.UWP
{
    // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh758325.aspx
    public class LocalFile : ILocalFile
    {
        public async Task SaveAsync(string filename, object obj)
        {
            if (obj is string)
            {
                string text = (string)obj;
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, text);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task<T> LoadAsync<T>(string filename)
        {
            if (typeof(T) == typeof(string))
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.GetFileAsync(filename);
                string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                return (T)(object)text;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Save(string FileName, object obj)
        {
            (SaveAsync(FileName, obj)).Wait();
        }

        public T Load<T>(string FileName)
        {
            Task<T> task = LoadAsync<T>(FileName);
            task.Wait();
            return task.Result;
        }
    }
}