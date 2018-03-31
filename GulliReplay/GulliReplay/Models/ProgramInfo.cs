using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.ObjectModel;

namespace GulliReplay
{
    public class ProgramInfo: IEquatable<ProgramInfo>, IComparable<ProgramInfo>
    {
        private object locker = new object();
        public ManualResetEvent EpisodeUpdatedEvent = new ManualResetEvent(false);

        private ObservableCollection<EpisodeInfo> _Episode = new ObservableCollection<EpisodeInfo>();
        [Ignore]
        public ObservableCollection<EpisodeInfo> Episode { get => _Episode; set { } }

        private object updateLocker = new object();
        public bool Updated = false;

        private bool _updating = false;
        public bool EnterUpdating()
        {
            lock (updateLocker)
            {
                if (!(_updating || Updated))
                {
                    _updating = true;
                    return true;
                }
                return false;
            }

        }

        public void LeaveUpdating(bool updated)
        {
            lock(updateLocker)
            {
                _updating = false;
                Updated = updated;
            }
        }

        [PrimaryKey]
        public string Url { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public ProgramInfo() { }
        public ProgramInfo(string url, string type, string name, string imageUrl)
        {
            Url = url;
            Type = type;
            Name = name;
            ImageUrl = imageUrl;
        }

        private List<EpisodeInfo> episodes = null;
        public List<EpisodeInfo> GetEpisodeList()
        {
            lock (locker)
            {
                if (episodes == null)
                    episodes = GulliDataSource.Default.GetEpisodeList(this);
            }
            return episodes;
        }

        public bool Equals(ProgramInfo other)
        {
            return Name == other.Name;
        }

        public int CompareTo(ProgramInfo other)
        {
            return Url.CompareTo(other.Url);
        }
    }
}
