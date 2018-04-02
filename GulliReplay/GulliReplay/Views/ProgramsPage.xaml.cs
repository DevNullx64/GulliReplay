using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace GulliReplay
{
    public partial class ProgramPage : ContentPage
    {
        ProgramsViewModel viewModel;

        public ProgramPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ProgramsViewModel(Progressbar);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var program = args.SelectedItem as ProgramInfo;
            if (program == null)
                return;

            await Navigation.PushAsync(new EpisodesPage(new EpisodesViewModel(program)));

            // Manually deselect item
            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.ProgramList.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}