using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace GulliReplay
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("http://gulli-replay-transmux.scdn.arkena.com/68857224948000/68857224948000_1500.mp4")));
        }

        public ICommand OpenWebCommand { get; }
    }
}