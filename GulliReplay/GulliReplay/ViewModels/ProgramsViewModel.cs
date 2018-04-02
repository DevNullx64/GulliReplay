using SQLite;
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

        public ObservableCollection<ProgramInfo> ProgramList { get; set; } = new ObservableCollection<ProgramInfo>();
        public Command LoadItemsCommand { get; set; }
        private ProgressBar Progress;

        public ProgramsViewModel(ProgressBar progress)
        {
            Title = defaultTitle + Helpers.updateString;
            Progress = progress;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Progress.IsVisible = true;
                Exception e = await GulliDataSource.Default.GetProgramList(ProgramList, Progress);
                if (e != null)
                    Title = defaultTitle + "(Update error)";
                else
                    Title = defaultTitle + "(" + ProgramList.Count.ToString() + ")";
                Progress.IsVisible = false;
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
