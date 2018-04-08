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

        public bool Equals(EpisodeInfo other)
        {
            return
                (Title == other.Title) &&
                (Saison == other.Saison) &&
                (Episode == other.Episode);
        }

        public int CompareTo(EpisodeInfo other)
        {
            int result = Saison.CompareTo(other.Saison);
            if (result == 0)
            {
                result = Episode.CompareTo(other.Episode);
                if (result == 0)
                {
                    result = Title.CompareTo(other.Title);
                }
            }

            return result;
        }
    }
}
