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
        public readonly EpisodeDataStore DataStore;

        public ObservableCollection<EpisodeInfo> EpisodeList { get; set; }
        public Command LoadEpisodeCommand { get; set; }

        public EpisodesViewModel(ProgramInfo program = null)
        {
            Title = program?.Name;
            EpisodeList = new ObservableCollection<EpisodeInfo>();
            DataStore = new EpisodeDataStore(program);
            LoadEpisodeCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                EpisodeList.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    EpisodeList.Add(item);
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
