using System;
using System.Collections.Generic;
using System.Text;

namespace GulliReplay
{
    public interface IReplayDataSource
    {
        List<ProgramInfo> GetProgramList();
        List<EpisodeInfo> GetEpisodeList(ProgramInfo program);
        Uri GetVideoStream(EpisodeInfo episode);
    }
}
