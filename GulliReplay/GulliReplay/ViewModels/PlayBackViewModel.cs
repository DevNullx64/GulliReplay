using Rox;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GulliReplay
{
    public class PlayBackViewModel: BaseViewModel
    {
        private readonly VideoView VideoView;
        private readonly EpisodeInfo EpisodeInfo;

        public PlayBackViewModel(VideoView videoView, EpisodeInfo episodeInfo)
        {
            VideoView = videoView;
            EpisodeInfo = episodeInfo;
        }

        public string EntrySource
        {
            get { return EpisodeInfo.GetVideoStream().ToString(); }
        }

        public ImageSource VideoSource
        {
            get
            {
                ImageSource result = null;
                ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                result = (ImageSource)imageSourceConverter.ConvertFromInvariantString(this.EntrySource);
                return result;
            }
        }
    }
}
