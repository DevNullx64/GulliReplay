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
        public readonly GulliDataSource DataStore = GulliDataSource.Default;
        public readonly Task OnEpisodeUpdated;

        public ObservableCollection<EpisodeInfo> EpisodeList { get; set; }
        public Command LoadEpisodeCommand { get; set; }
        private ProgramInfo Program;

        public EpisodesViewModel(ProgramInfo program = null)
        {
            Program = program;
            if (Program != null)
            {
                EpisodeList = new ObservableCollection<EpisodeInfo>();
                LoadEpisodeCommand = new Command(async () => await ExecuteLoadItemsCommand());
                OnEpisodeUpdated = new Task(() =>
                {
                    Program.EpisodeUpdatedEvent.WaitOne();
                    LoadEpisodeCommand.Execute(null);
                    Title = Program.Name;
                });

                if (!Program.EpisodeUpdatedEvent.WaitOne(0))
                {
                    Title = Program.Name + Helpers.updateString;
                    OnEpisodeUpdated.Start();
                }
                else
                {
                    Title = Program.Name;
                }
            }
        }

#pragma warning disable CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone. Utilisez l'opérateur 'await' pour attendre les appels d'API non bloquants ou 'await Task.Run(…)' pour effectuer un travail utilisant le processeur sur un thread d'arrière-plan.
        async Task ExecuteLoadItemsCommand()
#pragma warning restore CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone. Utilisez l'opérateur 'await' pour attendre les appels d'API non bloquants ou 'await Task.Run(…)' pour effectuer un travail utilisant le processeur sur un thread d'arrière-plan.
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                EpisodeList.Clear();
                var items = DataStore.GetEpisodeList(Program);
                foreach (var item in items)
                {
                    EpisodeList.SortedAdd(item);
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
