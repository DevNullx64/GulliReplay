using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace GulliReplay
{
    public interface IReplayDataSource
    {
        Task<Exception> GetProgramList(ObservableCollection<ProgramInfo> programs);
        Task<Exception> GetEpisodeList(ProgramInfo program);
        Uri GetVideoStream(EpisodeInfo episode);
    }
}
