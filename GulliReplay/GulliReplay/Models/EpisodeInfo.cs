using System;
using System.Collections.Generic;
using System.Text;

namespace GulliReplay
{
    public class EpisodeInfo: IEquatable<EpisodeInfo>, IComparable<EpisodeInfo>
    {
        public string Id { get; private set; }
        public ProgramInfo Program { get; private set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string SaisonEpisode
        {
            get => "Saison: " + Saison.ToString() + ", Episode: " + Episode.ToString();
            set { }
        }
        public byte Saison { get; set; }
        public byte Episode { get; set; }

        public EpisodeInfo()
            : this(null, "", "", "", 0, 0) { }
        public EpisodeInfo(ProgramInfo program, string id, string title, string imageUrl, byte saison, byte episode)
        {
            Program = program;
            Id = id;
            Title = title;
            ImageUrl = imageUrl;
            Saison = saison;
            Episode = episode;
        }

        public Uri GetVideoStream()
        {
            return Program?.DataSource.GetVideoStream(this);
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
            }

            return result;
        }
    }
}
