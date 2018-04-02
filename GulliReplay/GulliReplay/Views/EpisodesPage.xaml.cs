using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GulliReplay
{
    public partial class EpisodesPage : ContentPage
    {
        EpisodesViewModel ViewModel;

        public EpisodesPage(ProgramInfo program = null)
        {
            InitializeComponent();
            BindingContext = ViewModel = new EpisodesViewModel(program);
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

            if (!ViewModel.IsUpdated)
                ViewModel.LoadEpisodeCommand.Execute(null);
        }
    }
}
