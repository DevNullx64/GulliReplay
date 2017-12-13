using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GulliReplay
{
    public partial class EpisodesPage : ContentPage
    {
        EpisodesViewModel viewModel;

        public EpisodesPage(EpisodesViewModel viewModel = null)
        {
            InitializeComponent();
            if (viewModel == null)
                viewModel = new EpisodesViewModel();
            BindingContext = this.viewModel = viewModel;
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var episode = args.SelectedItem as EpisodeInfo;
            if (episode == null)
                return;

            //await Task.Run(() => Device.OpenUri(episode.GetVideoStream()));
            await Navigation.PushAsync(new PlayBack(episode));

            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.EpisodeList.Count == 0)
                viewModel.LoadEpisodeCommand.Execute(null);
        }
    }
}
