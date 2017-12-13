using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace GulliReplay
{
    public class ProgramsViewModel : BaseViewModel
    {
        public readonly ProgramDataStore DataStore;

        public ObservableCollection<ProgramInfo> ProgramList { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ProgramsViewModel()
        {
            Title = "Browse";
            ProgramList = new ObservableCollection<ProgramInfo>();
            DataStore = new ProgramDataStore(GulliDataSource.Default);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ProgramList.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    ProgramList.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
