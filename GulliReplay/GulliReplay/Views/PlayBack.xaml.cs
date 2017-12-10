using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using GulliReplay.Models;

namespace GulliReplay.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayBack : ContentPage
	{
        public PlayBack(EpisodeInfo item)
        {
            InitializeComponent();
            BindingContext = item;
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            CrossMediaManager.Current.Pause();
            pause.IsEnabled = false;
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            CrossMediaManager.Current.Stop();
            pause.IsEnabled = false;
            stop.IsEnabled = false;
        }

        private async void OnPlayClicked(object sender, EventArgs e)
        {
            await CrossMediaManager.Current.VideoPlayer.Play();
            pause.IsEnabled = true;
            stop.IsEnabled = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var item = (EpisodeInfo)BindingContext;
            CrossMediaManager.Current.Stop();
            CrossMediaManager.Current.StatusChanged += CurrentOnStatusChanged;
            CrossMediaManager.Current.PlayingChanged += OnPlayingChanged;
            player.Source = item.GetVideoStream().ToString();
            play.Clicked += OnPlayClicked;
            stop.Clicked += OnStopClicked;
            pause.Clicked += OnPauseClicked;
            player.AspectMode = VideoAspectMode.AspectFit;
        }

        private void OnPlayingChanged(object sender, PlayingChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                progress.Progress = e.Progress;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CrossMediaManager.Current.Stop();
            play.Clicked -= OnPlayClicked;
            stop.Clicked -= OnStopClicked;
            pause.Clicked -= OnPauseClicked;
            CrossMediaManager.Current.StatusChanged -= CurrentOnStatusChanged;
            CrossMediaManager.Current.PlayingChanged -= OnPlayingChanged;
        }

        private void CurrentOnStatusChanged(object sender, StatusChangedEventArgs statusChangedEventArgs)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var status = statusChangedEventArgs.Status;
                switch (status)
                {
                    case MediaPlayerStatus.Stopped:
                        pause.IsEnabled = false;
                        stop.IsEnabled = false;
                        StatusLabel.Text = "Stopped";
                        await StatusLabel.FadeTo(1);
                        break;
                    case MediaPlayerStatus.Paused:
                        pause.IsEnabled = true;
                        stop.IsEnabled = true;
                        StatusLabel.Text = "Paused";
                        await StatusLabel.FadeTo(1);
                        break;
                    case MediaPlayerStatus.Playing:
                        pause.IsEnabled = true;
                        stop.IsEnabled = true;
                        await StatusLabel.FadeTo(0);
                        break;
                    case MediaPlayerStatus.Loading:
                        pause.IsEnabled = false;
                        stop.IsEnabled = false;
                        StatusLabel.Text = "Loading";
                        await StatusLabel.FadeTo(1);
                        break;
                    case MediaPlayerStatus.Buffering:
                        pause.IsEnabled = false;
                        stop.IsEnabled = true;
                        StatusLabel.Text = "Buffering";
                        await StatusLabel.FadeTo(1);
                        break;
                    case MediaPlayerStatus.Failed:
                        pause.IsEnabled = false;
                        stop.IsEnabled = false;
                        StatusLabel.Text = "Failed";
                        await StatusLabel.FadeTo(1);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}