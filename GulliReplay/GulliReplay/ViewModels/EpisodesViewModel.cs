using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GulliReplay
{
    public class EpisodesViewModel : BaseViewModel
    {
        private ProgramInfo Program;

        public ObservableCollection<EpisodeInfo> EpisodeList { get; set; }
        public bool IsUpdated { get => Program.IsUpdated; set { } }
        public double Progress { get => Program.Progress; set { } }

        public Command LoadEpisodeCommand { get; set; }

        public EpisodesViewModel(ProgramInfo program = null)
        {
            this.Program = program;
            LoadEpisodeCommand = new Command(async () => await ExecuteLoadItemsCommand());
            if (program == null)
                EpisodeList = new ObservableCollection<EpisodeInfo>();
            else
            {
                Program.PropertyChanged += PropertyChangedEventHandler;
                IsBusy = Program.IsUpdating;
                EpisodeList = program.Episodes;
                Title = program.Name + " (" + EpisodeList.Count.ToString() + ")";
            }
        }
        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsUpdating")
                IsBusy = Program.IsUpdating;
            else
                OnPropertyChanged(e.PropertyName);
        }
        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Exception result = await GulliDataSource.Default.GetEpisodeList(Program, (p) => Program.Progress = p);
                if (result != null)
                    Title = Program.Name + "(" + result.Message + ")";
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