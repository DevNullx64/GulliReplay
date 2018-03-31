using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using SQLite;
using System.Diagnostics;
using System.Threading;
using System.Collections.ObjectModel;

namespace GulliReplay
{
    public class GulliDataSource : IReplayDataSource
    {
        private static readonly Uri ProgramPage = new Uri("http://replay.gulli.fr/all");

        private SQLiteConnection db;
        public static GulliDataSource Default = new GulliDataSource();

        public ManualResetEvent ProgramUpdated = new ManualResetEvent(false);

        public GulliDataSource()
        {
            try
            {
                var databasePath = Path.Combine(LocalFile.Root, "Gulli.db");
                db = new SQLiteConnection(databasePath);
                db.CreateTable<ProgramInfo>();
                db.CreateTable<EpisodeInfo>();
            }
            catch (Exception e)
            {

            }
        }

        private object insertLocker = new object();
        private object selectLocer = new object();
        public int DbInsert(object item)
        {
            lock(insertLocker)
                return db.Insert(item);
        }

        private bool PogrameUpdating = false;
        private object ProgramLock = new object();

        public void GetProgramList(ObservableCollection<ProgramInfo> programs, BaseViewModel model)
        {
            lock(ProgramLock){
                if (PogrameUpdating) return;
                PogrameUpdating = true;
            }

            model.IsBusy = true;
            TableQuery<ProgramInfo> query;
            lock (selectLocer)
                query = db.Table<ProgramInfo>();

            if (query != null)
            {
                programs.SortedAdd(query);

                new Task(() =>
                {
                    try
                    {
                        Regex ProgramRegex =
                        new Regex(@"(<div\s+class=""wrap-img\s+program""\s*>" +
                        @"\s*<a\s+href=""(?<url>http://replay\.gulli\.fr/(?<type>[^/]+)/[^""]+)""\s*>" +
                        @"\s*<img\s+src=""(?<img>http://[a-z1-9]+-gulli\.ladmedia\.fr/r/[^""]+/img/var/storage/imports/(?<filename>[^""]+))""\s*alt=""(?<name>[^""]+)""\s*/>" +
                        @"\s*</a>\s*</div>)", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

                        MatchCollection matches = ProgramRegex.Matches(ProgramPage.GetContent());
                        for (int i = 0; i < matches.Count; i++)
                        {
                            Match m = matches[i];
                            string s = m.Value;

                            string url = m.Groups["url"].Value;
                            var pgm = query.Where((p) => p.Url == url);
                            ProgramInfo program;
                            if (pgm.Count() == 0)
                            {
                                program = new ProgramInfo(
                                        m.Groups["url"].Value,
                                        m.Groups["type"].Value,
                                        WebUtility.HtmlDecode(m.Groups["name"].Value),
                                        GetImage(new Uri(GetImageUrl(m.Groups["filename"].Value))));
                                DbInsert(program);
                                programs.Add(program);
                            }
                        }
                        ProgramUpdated.Set();

                        foreach (ProgramInfo p in programs)
                            GetEpisodeList(p, null);

                    }
                    catch (Exception e)
                    {
                        Debug.Write(e.Message);
                    }

                }).Start();
            }
        }

        private void GetEpisodeList(ProgramInfo program, List<EpisodeInfo> result = null)
        {
            if (program.EnterUpdating())
            {
                try
                {
                    TableQuery<EpisodeInfo> query;
                    lock (selectLocer)
                        query = db.Table<EpisodeInfo>().Where((e) => e.ProgramUrl == program.Url);

                    Regex EpisodeRegex = new Regex(
                            @"<a href=""" + program.Url + @"/VOD(?<vid>\d+)""><img class=""img-responsive""\s+src=""(?<img>[^""]+/img/var/storage/imports/(?<filename>[^""]+))""/><span\s+class=""title"">" +
                            @"<span>Saison (?<saison>\d+)\s*,\s*&Eacute;pisode\s*(?<episode>\d+)</span>(?<title>[^<]+)</span></a>",
                            RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

                    WebRequest request = HttpWebRequest.Create(program.Url);
                    string content = request.GetContent();
                    MatchCollection matches = EpisodeRegex.Matches(content);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        Match m = matches[i];

                        string vid = m.Groups["vid"].Value;
                        var epd = query.Where((e) => e.Id == vid);

                        if (epd.Count() == 0)
                        {
                            EpisodeInfo episode = new EpisodeInfo(
                                program,
                                vid,
                                WebUtility.HtmlDecode(m.Groups["title"].Value.Replace("\n", "").Trim()),
                                GetImage(new Uri(GetImageUrl(m.Groups["filename"].Value))),
                                byte.Parse(m.Groups["saison"].Value),
                                byte.Parse(m.Groups["episode"].Value));

                            DbInsert(episode);
                            result?.Add(episode);
                        }

                    }

                    Debug.Write(program.Name + ": Updated");
                    program.LeaveUpdating(true);
                }
                catch (Exception e)
                {
                    Debug.Write(program.Name);
                    Debug.Write(e.Message);
                    program.LeaveUpdating(false);
                }

                result?.Sort();
                program.EpisodeUpdatedEvent.Set();
            }
        }

        public List<EpisodeInfo> GetEpisodeList(ProgramInfo program)
        {
            var query = db.Table<EpisodeInfo>().Where((e) => e.ProgramUrl == program.Url);
            List<EpisodeInfo> result = new List<EpisodeInfo>(query);

            if (!program.Updated)
                new Task(() => GetEpisodeList(program, result)).Start();

            result.Sort();
            return result;
        }

        public enum GulliQuality
        {
            _200 = 200,
            _350 = 350,
            _750 = 750,
            _900 = 900,
            _1500 = 1500
        }

        public Uri GetVideoStream(EpisodeInfo episode) => GetVideoStream(episode, GulliQuality._900);
        public Uri GetVideoStream(EpisodeInfo episode, GulliQuality quality)
        {
            int q = (int)quality;
            string str = string.Format("http://gulli-replay-transmux.scdn.arkena.com/{0}/{0}_{1}.mp4", episode.Id, q);
            return new Uri(str);
        }
        static int imgHeigth = App.DisplayScreenWidth >> 1;
        static int imgWidth = (imgHeigth >> 2) * 3;

        public string GetImageUrl(string image)
        {
            return "http://resize-gulli.ladmedia.fr/r/" + imgHeigth.ToString() + "," + imgWidth.ToString() + ",smartcrop,center-top/img/var/storage/imports/" + image;
        }

        public string GetImage(Uri uri)
        {
            string result = Path.Combine(LocalFile.Root, Path.GetFileName(uri.ToString()));
            int retry = 3;
            if (!File.Exists(result))
                while (!Helpers.Download(uri, result) && (retry > 0))
                    retry--;
            return (retry > 0) ? result : uri.ToString();
        }

        public List<ProgramInfo> GetProgramList()
        {
            throw new NotImplementedException();
        }
    }
}