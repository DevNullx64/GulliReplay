using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GulliReplay
{
    public class EpisodesViewModel : BaseViewModel
    {
        private ProgramInfo Program;

        public ObservableCollection<EpisodeInfo> EpisodeList { get; set; }
        double progress = 0;
        public double Progress
        {
            get { return progress; }
            set { SetProperty(ref progress, value); }
        }

        public Command LoadEpisodeCommand { get; set; }

        public EpisodesViewModel(ProgramInfo program = null)
        {
            Program = program;
            LoadEpisodeCommand = new Command(async () => await ExecuteLoadItemsCommand());
            if (Program == null)
                EpisodeList = new ObservableCollection<EpisodeInfo>();
            else
            {
                EpisodeList = program.Episodes;
                Title = Program.Name;
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await GulliDataSource.Default.GetEpisodeList(Program, (p) => Progress = p);
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
