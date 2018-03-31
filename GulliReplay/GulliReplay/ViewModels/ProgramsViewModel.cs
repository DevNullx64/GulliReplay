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
        public readonly Task OnProgramUpdated;

        public ObservableCollection<ProgramInfo> ProgramList { get; set; } = new ObservableCollection<ProgramInfo>();
        public Command LoadItemsCommand { get; set; }

        public ProgramsViewModel()
        {
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

#pragma warning disable CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone. Utilisez l'opérateur 'await' pour attendre les appels d'API non bloquants ou 'await Task.Run(…)' pour effectuer un travail utilisant le processeur sur un thread d'arrière-plan.
        async Task ExecuteLoadItemsCommand()
#pragma warning restore CS1998 // Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone. Utilisez l'opérateur 'await' pour attendre les appels d'API non bloquants ou 'await Task.Run(…)' pour effectuer un travail utilisant le processeur sur un thread d'arrière-plan.
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                GulliDataSource.Default.GetProgramList(ProgramList, this);
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
