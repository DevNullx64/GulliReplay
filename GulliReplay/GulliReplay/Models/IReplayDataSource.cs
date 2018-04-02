using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GulliReplay
{
    public interface IReplayDataSource
    {
        Task<Exception> GetProgramList(ObservableCollection<ProgramInfo> programs, Action<double> onProgress);
        Task<Exception> GetEpisodeList(ProgramInfo program, Action<double> onProgress);
        Uri GetVideoStream(EpisodeInfo episode);
    }
}
