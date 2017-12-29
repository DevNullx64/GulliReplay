using UIKit;

namespace GulliReplay.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
            App.DisplayScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
            App.DisplayScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.DisplayScaleFactor = (double)UIScreen.MainScreen.Scale;
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
        }
	}
}
