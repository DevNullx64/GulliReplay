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
        public readonly Task OnEpisodeUpdated;

        public ObservableCollection<EpisodeInfo> EpisodeList { get; set; }
        public Command LoadEpisodeCommand { get; set; }

        public EpisodesViewModel(ProgramInfo program = null)
        {
            if (program != null)
            {
                EpisodeList = new ObservableCollection<EpisodeInfo>();
                DataStore = new EpisodeDataStore(program);
                LoadEpisodeCommand = new Command(async () => await ExecuteLoadItemsCommand());
                OnEpisodeUpdated = new Task(() =>
                {
                    program.EpisodeUpdatedEvent.WaitOne();
                    LoadEpisodeCommand.Execute(null);
                    Title = program.Name;
                });

                if (!program.EpisodeUpdatedEvent.WaitOne(0))
                {
                    Title = program.Name + Helpers.updateString;
                    OnEpisodeUpdated.Start();
                }
                else
                {
                    Title = program.Name;
                }
            }
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
