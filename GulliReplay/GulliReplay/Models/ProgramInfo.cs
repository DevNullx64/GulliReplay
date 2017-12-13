using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace GulliReplay
{
    public class ProgramInfo
    {
        internal readonly ReplayDataSource DataSource;
        public string Id { get; private set; }
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string ImageUrl { get; private set; }

        public ProgramInfo(ReplayDataSource dataSource, string type, string name, string url, string imageUrl)
        {
            DataSource = dataSource;
            Id = Guid.NewGuid().ToString();
            Type = type;
            Name = name;
            Url = url;
            ImageUrl = imageUrl;
        }

        public List<EpisodeInfo> GetEpisodeList()
        {
            return DataSource.GetEpisodeList(this);
        }
    }
}
