﻿using SQLite;
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

        public ProgramsViewModel()
        {
            Title = defaultTitle + Helpers.updateString;
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Exception e = await GulliDataSource.Default.GetProgramList(ProgramList);
                if (e != null)
                    Title = defaultTitle + "(Update error)";
                else
                    Title = defaultTitle + "(" + ProgramList.Count.ToString() + ")";
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
