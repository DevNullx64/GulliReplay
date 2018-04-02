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
using Xamarin.Forms;

namespace GulliReplay
{
    public class GulliDataSource : IReplayDataSource
    {
        public const string FilmProgramName = "# Films";

        private static readonly Uri ProgramPage = new Uri("http://replay.gulli.fr/all");

        private SQLiteConnection db;
        public static GulliDataSource Default = new GulliDataSource();

        public GulliDataSource()
        {
            var databasePath = Path.Combine(LocalFile.Root, "Gulli.db");
            db = new SQLiteConnection(databasePath);
            db.CreateTable<ProgramInfo>();
            db.CreateTable<EpisodeInfo>();
        }

        private object Locker = new object();
        public int DbInsert(object item)
        {
            lock (Locker)
                return db.Insert(item);
        }
        public int DbUpdate(object item)
        {
            lock (Locker)
                return db.Update(item);
        }

        private bool PogrameUpdating = false;
        private object ProgramLock = new object();

        public Exception GetProgramListSync(ObservableCollection<ProgramInfo> programs, Action<double> onProgress = null)
        {
            try
            {
                lock (ProgramLock)
                {
                    if (PogrameUpdating)
                    {
                        onProgress?.Invoke(1);
                        return null;
                    }
                    else
                    {
                        onProgress?.Invoke(0);
                        PogrameUpdating = true;
                    }
                }

                TableQuery<ProgramInfo> query = null;
                lock (Locker)
                    query = db.Table<ProgramInfo>();

                if (query != null)
                    programs.SortedAdd(query);

                Regex ProgramRegex =
                new Regex(@"(<div\s+class=""wrap-img\s+program""\s*>" +
                @"\s*<a\s+href=""(?<url>http://replay\.gulli\.fr/(?<type>[^/]+)/[^""]+)""\s*>" +
                @"\s*<img\s+src=""(?<img>http://[a-z1-9]+-gulli\.ladmedia\.fr/r/[^""]+/img/var/storage/imports/(?<filename>[^""]+))""\s*alt=""(?<name>[^""]+)""\s*/>" +
                @"\s*</a>\s*</div>)", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

                MatchCollection matches = ProgramRegex.Matches(ProgramPage.GetContent());
                for (int i = 0; i < matches.Count; i++)
                {
                    onProgress?.Invoke((double)i / matches.Count);

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
                        programs.SortedAdd(program);
                    }
                }

                Task.Run(() =>
                    {
                        ObservableCollection<EpisodeInfo> films = null;
                        for (int i = 0; i < programs.Count; i++)
                        {
                            ProgramInfo program = programs[i];
                            GetEpisodeListSync(program, (p) => program.Progress = p);
                            if (program.IsFilm)
                            {
                                program.Name = GulliDataSource.FilmProgramName;
                                DbUpdate(program);
                                if (films == null)
                                {
                                    films = program.Episodes;
                                    programs.RemoveAt(i);
                                    programs.SortedAdd(program);
                                }
                                else
                                {
                                    films.SortedAdd(program.Episodes);
                                    programs.RemoveAt(i--);
                                }
                            }
                        }
                    }
                );

            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return e;
            }
            finally { PogrameUpdating = false; }

            return null;
        }
        public async Task<Exception> GetProgramList(ObservableCollection<ProgramInfo> programs, Action<double> onProgress = null)
        {
            Exception result = null;
            await Task.Run(() => result = GetProgramListSync(programs, onProgress));
            return result;
        }

        public Exception GetEpisodeListSync(ProgramInfo program, Action<double> onProgress = null)
        {
            if (program.EnterUpdating())
            {
                try
                {
                    onProgress?.Invoke(0);

                    TableQuery<EpisodeInfo> query;
                    TableQuery<ProgramInfo> programs;
                    lock (Locker)
                    {
                        programs = db.Table<ProgramInfo>().Where((e) => e.Name == program.Name);
                        List<string> urls = new List<string>();
                        foreach (ProgramInfo pgm in programs)
                            urls.Add(pgm.Url);
                        query = db.Table<EpisodeInfo>().Where((e) => urls.Contains(e.ProgramUrl));
                    }

                    double progress = 0.0;
                    foreach (ProgramInfo pgm in programs)
                    {
                        Regex EpisodeRegex = new Regex(
                            @"<a href=""" + pgm.Url + @"/VOD(?<vid>\d+)""><img class=""img-responsive""\s+src=""(?<img>[^""]+/img/var/storage/imports/(?<filename>[^""]+))""/><span\s+class=""title"">" +
                            @"<span>Saison (?<saison>\d+)\s*,\s*&Eacute;pisode\s*(?<episode>\d+)</span>(?<title>[^<]+)</span></a>",
                            RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

                        WebRequest request = HttpWebRequest.Create(pgm.Url);
                        string content = request.GetContent();
                        MatchCollection matches = EpisodeRegex.Matches(content);
                        for (int i = 0; i < matches.Count; i++)
                        {
                            progress += 1.0 / matches.Count;
                            onProgress?.Invoke(progress / programs.Count());
                            Match m = matches[i];

                            string vid = m.Groups["vid"].Value;
                            var epd = query.Where((e) => e.Id == vid);

                            EpisodeInfo episode;
                            if (epd.Count() == 0)
                            {
                                episode = new EpisodeInfo(
                                    pgm.Url,
                                    vid,
                                    WebUtility.HtmlDecode(m.Groups["title"].Value.Replace("\n", "").Trim()),
                                    GetImage(new Uri(GetImageUrl(m.Groups["filename"].Value))),
                                    byte.Parse(m.Groups["saison"].Value),
                                    byte.Parse(m.Groups["episode"].Value));

                                DbInsert(episode);
                            }
                            else
                                episode = epd.First();
                            program.Episodes.SortedAdd(episode);
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
                    return e;
                }
            }
            return null;
        }
        public async Task<Exception> GetEpisodeList(ProgramInfo program, Action<double> onProgress = null)
        {
            Exception result = null;
            await Task.Run(() => { result = GetEpisodeListSync(program, onProgress); });
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