using SQLite;
using System;

namespace GulliReplay
{
    public class EpisodeInfo : IEquatable<EpisodeInfo>, IComparable<EpisodeInfo>
    {
        [PrimaryKey]
        public string Id { get; set; }
        [Indexed]
        public string ProgramUrl { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public byte Saison { get; set; }
        public byte Episode { get; set; }
        [Ignore]
        public bool IsEpisode { get => Episode != 0 && Saison != 0; set { } }

        public EpisodeInfo() { }
        public EpisodeInfo(string programUrl, string id, string title, string imageUrl, byte saison, byte episode)
        {
            ProgramUrl = programUrl;
            Id = id;
            Title = title;
            Saison = saison;
            Episode = episode;
            ImageUrl = imageUrl;
       }

        public Uri GetVideoStream()
        {
            return GulliDataSource.Default.GetVideoStream(this);
        }

        public bool Equals(EpisodeInfo other) => CompareTo(other) == 0;
        public int CompareTo(EpisodeInfo other)
        {
            int result = Saison.CompareTo(other.Saison);
            if (result == 0)
                result = Episode.CompareTo(other.Episode);
            if (result == 0)
                result = Title.CompareTo(other.Title);

            return result;
        }

        public override string ToString()
        {
            return ProgramUrl + " | [" + Saison.ToString() + "x" + Episode.ToString() + "] " + Title;
        }
    }
}
