using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace GulliReplay
{
    class GulliDataSource : ReplayDataSource
    {
        private static readonly Uri ProgramPage = new Uri("http://replay.gulli.fr/all");
        public static GulliDataSource Default = new GulliDataSource();

        public List<ProgramInfo> GetProgramList()
        {
            Regex ProgramRegex =
                new Regex(@"(<div\s+class=""wrap-img\s+program""\s*>" +
                @"\s*<a\s+href=""(?<url>http://replay\.gulli\.fr/(?<type>[^/]+)/[^""]+)""\s*>" +
                @"\s*<img\s+src=""(?<img>http://[a-z1-9]+-gulli\.ladmedia\.fr/[^""]+)""\s*alt=""(?<name>[^""]+)""\s*/>" +
                @"\s*</a>\s*</div>)", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

            MatchCollection matches = ProgramRegex.Matches(ProgramPage.GetContent());
            List<ProgramInfo> result = new List<ProgramInfo>(matches.Count);
            for (int i = 0; i < matches.Count; i++)
            {
                Match m = matches[i];
                string s = m.Value;
                result.Add(
                    new ProgramInfo(
                        this,
                        m.Groups["type"].Value,
                        WebUtility.HtmlDecode(m.Groups["name"].Value),
                        m.Groups["url"].Value,
                        m.Groups["img"].Value));
            }
            return result;
        }

        public List<EpisodeInfo> GetEpisodeList(ProgramInfo program)
        {
            Regex EpisodeRegex = new Regex(
                    @"<a href=""" + program.Url + @"/VOD(?<vid>\d+)""><img class=""img-responsive""\s+src=""(?<img>[^""]+)""/><span\s+class=""title"">" +
                    @"<span>Saison (?<saison>\d+)\s*,\s*&Eacute;pisode\s*(?<episode>\d+)</span>(?<title>[^<]+)</span></a>",
                    RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

            MatchCollection matches = EpisodeRegex.Matches(HttpWebRequest.Create(program.Url).GetContent());
            List<EpisodeInfo> result = new List<EpisodeInfo>(matches.Count);
            for (int i = 0; i < matches.Count; i++)
            {
                Match m = matches[i];
                result.Add(
                    new EpisodeInfo(
                        program,
                        m.Groups["vid"].Value,
                        WebUtility.HtmlDecode(m.Groups["title"].Value.Replace("\n", "").Trim()),
                        m.Groups["img"].Value,
                        byte.Parse(m.Groups["saison"].Value),
                        byte.Parse(m.Groups["episode"].Value)));
            }

            return result;
        }

        public Uri GetVideoStream(EpisodeInfo episode)
        {
            int quality = 1500;
            string str = string.Format("http://gulli-replay-transmux.scdn.arkena.com/{0}/{0}_{1}.mp4", episode.Id, quality);
            return new Uri(str);
        }
    }
}
