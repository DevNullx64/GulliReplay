using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace GulliReplay
{
    public class ProgramsViewModel : BaseViewModel
    {
        private const string defaultTitle = "Choisis ta série";
        public readonly ProgramDataStore DataStore;
        public readonly Task OnProgramUpdated;

        public ObservableCollection<ProgramInfo> ProgramList { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ProgramsViewModel()
        {
            ProgramList = new ObservableCollection<ProgramInfo>();
            DataStore = new ProgramDataStore(GulliDataSource.Default);
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            OnProgramUpdated = new Task(() =>
            {
                GulliDataSource.Default.ProgramUpdated.WaitOne();
                LoadItemsCommand.Execute(null);
                Title = defaultTitle;
            });

            if (!GulliDataSource.Default.ProgramUpdated.WaitOne(0))
            {
                Title = defaultTitle + Helpers.updateString;
                OnProgramUpdated.Start();
            }
            else
            {
                Title = defaultTitle;
            }
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
