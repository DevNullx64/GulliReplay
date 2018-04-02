using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GulliReplay
{
    public class ProgramInfo : IEquatable<ProgramInfo>, IComparable<ProgramInfo>, INotifyPropertyChanged
    {

        [Ignore]
        public ObservableCollection<EpisodeInfo> Episodes { get; set; } = new ObservableCollection<EpisodeInfo>();

        private object updateLocker = new object();
        public bool Updated = false;

        private bool _updating = false;
        [Ignore]
        public bool IsUpdating { get => _updating || !Updated; set { } }
        private double _Progress = 0;
        [Ignore]
        public double Progress
        {
            get => _Progress;
            set => SetProperty(ref _Progress, value);
        }

        public bool EnterUpdating()
        {
            lock (updateLocker)
            {
                if (!(_updating || Updated))
                {
                    SetProperty(ref _updating, true, "IsUpdating");
                    return true;
                }
                return false;
            }
        }
        public void LeaveUpdating(bool updated)
        {
            lock (updateLocker)
            {
                SetProperty(ref _updating, false, "IsUpdating");
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

        public void GetEpisodeList()
        {
            GulliDataSource.Default.GetEpisodeListSync(this);
        }

        public bool Equals(ProgramInfo other)
        {
            return Name == other.Name;
        }

        public int CompareTo(ProgramInfo other)
        {
            int result = Name.CompareTo(other.Name);
            if (result == 0)
                result = Url.CompareTo(other.Url);
            return result;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}