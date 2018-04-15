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
        public ObservableSortedCollection<EpisodeInfo> Episodes { get; set; } = new ObservableSortedCollection<EpisodeInfo>();

        private object updateLocker = new object();

        [Ignore]
        public bool IsUpdated => isUpdated;
        private bool isUpdated = false;

        [Ignore]
        public bool IsVisible { get => isUpdating || !isUpdated; set { } }

        [Ignore]
        public bool IsUpdating { get => isUpdating; set { } }
        private bool isUpdating = false;

        [Ignore]
        public double Progress
        {
            get => progress;
            set => SetProperty(ref progress, value);
        }
        private double progress = 0;

        public bool EnterUpdating()
        {
            lock (updateLocker)
            {
                if (!isUpdating)
                {
                    SetProperty(ref isUpdating, true, "IsUpdating");
                    return true;
                }
                else
                    return false;
            }
        }
        public void LeaveUpdating(bool updated)
        {
            lock (updateLocker)
            {
                SetProperty(ref isUpdated, updated, "IsUpdated");
                SetProperty(ref isUpdating, !updated, "IsUpdating");
            }
        }

        [PrimaryKey]
        public string Url { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        [Ignore]
        internal bool IsMovie => ((Episodes.Count == 1) && (!Episodes[0].IsEpisode));

        public ProgramInfo() { }
        public ProgramInfo(string url, string type, string name, string imageUrl)
        {
            Url = url;
            Name = name;
            Type = type;
            ImageUrl = imageUrl;
        }

        public bool Equals(ProgramInfo other) => Name == other.Name;

        public int CompareTo(ProgramInfo other)
        {
            return Name.CompareTo(other.Name); ;
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
            if ((propertyName == "IsUpdated") || (propertyName == "IsUpdating"))
                OnPropertyChanged("IsVisible");
        }

        public override string ToString()=> Url;
    }
}