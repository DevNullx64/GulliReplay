using Plugin.Connectivity;
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
        private Page Parent;
        private const string defaultTitle = "Choisis ta série";

        public ObservableCollection<ProgramInfo> ProgramList { get; set; } = new ObservableCollection<ProgramInfo>();
        public Command LoadItemsCommand { get; set; }
        double progress = 0;
        public double Progress
        {
            get { return progress; }
            set { SetProperty(ref progress, value); }
        }

        public ProgramsViewModel(Page parent)
        {
            Parent = parent;
            Title = defaultTitle;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    await Parent.DisplayAlert("Pas de connection", "GulliReplay necessite une connection internet pour pouvoir fonctionner", "Ok");
                }
                else
                {
                    Exception e = await GulliDataSource.Default.GetProgramList(ProgramList, (p) => Progress = p);
                    if (e != null)
                        Title = defaultTitle + " (Erreur)";
                    else
                        Title = defaultTitle + " (" + ProgramList.Count.ToString() + ")";
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
