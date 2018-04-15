using FormsVideoLibrary;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GulliReplay
{
    public class PlayBackViewModel: BaseViewModel
    {
        private readonly EpisodeInfo EpisodeInfo;

        public PlayBackViewModel(EpisodeInfo episodeInfo)
        {
            EpisodeInfo = episodeInfo;
        }

        public VideoSource VideoSource
        {
            get
            {
                return VideoSource.FromUri(EpisodeInfo.GetVideoStream().ToString());
            }
        }
    }
}
