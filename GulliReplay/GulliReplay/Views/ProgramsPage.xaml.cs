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
            BindingContext = viewModel = new ProgramsViewModel(this);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var program = args.SelectedItem as ProgramInfo;
            if (program == null)
                return;

            await Navigation.PushAsync(new EpisodesPage(program));

            // Manually deselect item
            ItemsListView.SelectedItem = null;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.ProgramList.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        string[] QualityLabel = new string[] {
            "Très basse (200Kb/s)",
            "Basse (350Kb/s)",
            "Moyenne (700Kb/s)",
            "Haute (900Kb/s)",
            "Maximal (1500Kb/s)" };

        public async void Parameters_Clicked(object sender, EventArgs e)
        {
            string answer = await DisplayActionSheet(
                "Quelle qualité veux-tu pour les vidéos ?",
                "Actuellement: " + Parameters.DefaultQuality.ToString().Substring(1) + "Kb/s", null,
                QualityLabel);

            for(int i = 0; i < QualityLabel.Length; i++)
            {
                if (QualityLabel[i] == answer)
                    Parameters.DefaultQuality = (GulliQuality)i;
                Parameters.Save();
            }
        }
    }
}