using GulliReplay.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ViewManagement;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarImplementation))]
namespace GulliReplay.UWP
{
    public class StatusBarImplementation : IStatusBar
    {
        public bool HideStatusBar()
        {
            try
            {
                StatusBar.GetForCurrentView().HideAsync().AsTask().Wait();
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }

        public bool ShowStatusBar()
        {
            try
            {
                StatusBar.GetForCurrentView().ShowAsync().AsTask().Wait();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
