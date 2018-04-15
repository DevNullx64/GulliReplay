using Xamarin.Forms;

namespace GulliReplay
{
    public partial class PlayBack : ContentPage
    {
        PlayBackViewModel playBackViewModel;

        public PlayBack(EpisodeInfo item)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            BindingContext = playBackViewModel = new PlayBackViewModel(item);
        }

        protected override void OnAppearing()
        {
            DependencyService.Get<IStatusBar>().HideStatusBar();
            VideoViewer.Play();
        }

        protected override void OnDisappearing()
        {
            DependencyService.Get<IStatusBar>().ShowStatusBar();
            NavigationPage.SetHasNavigationBar(this, false);
            VideoViewer.Stop();
        }
    }
}