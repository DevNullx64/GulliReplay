using Rox;
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
            playBackViewModel= new PlayBackViewModel(VideoViewer, item);
            BindingContext = playBackViewModel;
        }

        public VideoView GetVideoView()
        {
            return VideoViewer;
        }

        protected override void OnAppearing()
        {
            DependencyService.Get<IStatusBar>().HideStatusBar();
        }

        protected override void OnDisappearing()
        {
            DependencyService.Get<IStatusBar>().ShowStatusBar();
            NavigationPage.SetHasNavigationBar(this, false);
            VideoViewer.Stop();
            BindingContext = null;
            playBackViewModel = null;
            grid.Children.Clear();
        }
    }
}