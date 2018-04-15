using Xamarin.Forms;

namespace GulliReplay
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new ProgramPage());
        }

        public static uint DisplayScreenWidth { get; internal set; }
        public static uint DisplayScreenHeight { get; internal set; }
        public static double DisplayScaleFactor { get; internal set; }
    }
}