using GulliReplay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GulliReplay.Services
{
    public interface ReplayDataSource
    {
        List<ProgramInfo> GetProgramList();
        List<EpisodeInfo> GetEpisodeList(ProgramInfo program);
        Uri GetVideoStream(EpisodeInfo episode);
    }
}
