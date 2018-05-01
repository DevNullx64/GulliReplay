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
using System.Linq;
using System.Collections;

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

        private object DbLocker = new object();
        public int DbInsert(object item)
        {
            lock (DbLocker) return db.Insert(item);
        }
        public int DbUpdate(object item)
        {
            lock (DbLocker) return db.Update(item);
        }

        private bool PogrameUpdating = false;
        private object ProgramLock = new object();

        public Exception GetProgramListSync(ObservableSortedCollection<ProgramInfo> programs, Action<double> onProgress = null)
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(programs.SynchronizationObject);

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
                lock (DbLocker)
                    query = db.Table<ProgramInfo>();

                if (query != null)
                    programs.Add(query);

                Regex ProgramRegex =
                new Regex(@"(<div\s+class=""wrap-img\s+program""\s*>" +
                @"\s*<a\s+href=""(?<url>http://replay\.gulli\.fr/(?<type>[^/]+)/[^""]+)""\s*>" +
                @"\s*<img\s+src=""(?<img>http://[a-z1-9]+-gulli\.ladmedia\.fr/r/[^""]+/img/var/storage/imports/(?<filename>[^""]+))""\s*alt=""(?<name>[^""]+)""\s*/>" +
                @"\s*</a>\s*</div>)", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

                string content = ProgramPage.GetContent();
                MatchCollection matches = ProgramRegex.Matches(content);
                List<Match> matcheList = new List<Match>(matches.Count);
                for (int i = 0; i < matches.Count; i++)
                    matcheList.Add(matches[i]);

                int count = 0;
                Parallel.ForEach(matcheList, (Match m) =>
                {
                    count++;
                    onProgress?.Invoke((double)count / matcheList.Count);
                    string s = m.Value;

                    string url = m.Groups["url"].Value;
                    if (query.Where((p) => p.Url == url).Count() == 0)
                    {
                        if (SynchronizationContext.Current == null)
                            SynchronizationContext.SetSynchronizationContext(programs.SynchronizationObject);

                        ProgramInfo program = new ProgramInfo(
                                m.Groups["url"].Value,
                                m.Groups["type"].Value,
                                WebUtility.HtmlDecode(m.Groups["name"].Value),
                                GetImageUrl(m.Groups["filename"].Value));
                        DbInsert(program);
                        programs.Add(program);
                    }
                });

                GetAllEpisode(programs);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return e;
            }
            finally { PogrameUpdating = false; }

            return null;
        }
        public async Task<Exception> GetProgramList(ObservableSortedCollection<ProgramInfo> programs, Action<double> onProgress = null)
        {
            Exception result = null;
            await Task.Run(() => result = GetProgramListSync(programs, onProgress));
            return result;
        }

        private object GetAllLocker = new object();
        private bool GetAll = false;
        public void GetAllEpisode(ObservableSortedCollection<ProgramInfo> programs)
        {
            Task.Run(() =>
            {
                try
                {
                    if (GetAll)
                        return;
                    GetAll = true;

                    ObservableSortedCollection<EpisodeInfo> films = null;
                    Parallel.ForEach(new List<ProgramInfo>(programs), (ProgramInfo program) =>
                    {
                        GetEpisodeListSync(program, (p) => program.Progress = p);
                        if (program.Episodes.Count == 0)
                        {
                            programs.Remove(program);
                        }
                        else if (program.IsMovie)
                        {
                            programs.Remove(program);
                            program.Name = GulliDataSource.FilmProgramName;
                            DbUpdate(program);
                            lock (GetAllLocker)
                                if (films == null)
                                {
                                    films = program.Episodes;
                                    programs.Add(program);
                                }
                                else
                                {
                                    films.Add(program.Episodes);
                                }
                        }
                    });
                }
                catch (Exception e)
                {
                    Debug.Write(e.Message);
                }
                GetAll = false;
            });
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
                    lock (DbLocker)
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
                            @"<a href=""" + pgm.Url + @"/(VOD)?(?<vid>\d+)""><img class=""img-responsive""\s+src=""(?<img>[^""]+/img/var/storage/imports/(?<filename>[^""]+))""/><span\s+class=""title"">" +
                            @"<span>Saison (?<saison>\d+)\s*,\s*&Eacute;pisode\s*(?<episode>\d+)</span>(?<title>[^<]+)</span></a>",
                            RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

                        HttpWebRequest request = HttpWebRequest.Create(pgm.Url) as HttpWebRequest;
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
                                    GetImageUrl(m.Groups["filename"].Value),
                                    byte.Parse(m.Groups["saison"].Value),
                                    byte.Parse(m.Groups["episode"].Value));

                                DbInsert(episode);
                            }
                            else
                                episode = epd.First();

                            program.Episodes.Add(episode);
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

        public Uri GetVideoStream(EpisodeInfo episode) => GetVideoStream(episode, Parameters.DefaultQuality);
        public Uri GetVideoStream(EpisodeInfo episode, GulliQuality quality)
        {
            string str = string.Format("http://gulli-replay-transmux.scdn.arkena.com/{0}/{0}{1}.mp4", episode.Id, quality);
            return new Uri(str);
        }
        static uint imgHeigth = App.DisplayScreenWidth >> 1;
        static uint imgWidth = (imgHeigth >> 2) * 3;

        public string GetImageUrl(string image)
        {
            string result = "http://resize-gulli.ladmedia.fr/r/" + imgHeigth.ToString() + "," + imgWidth.ToString() + ",smartcrop,center-top/img/var/storage/imports/" + image;
            GetImage(result);
            return result;
        }

        public string GetImage(string uri) => GetImage(new Uri(uri));
        public string GetImage(Uri uri)
        {
            string result = Path.Combine(LocalFile.Root, Path.GetFileName(uri.ToString()));
            if (File.Exists(result))
                return result;
            else
            {
                Helpers.BackgroundDownloader(uri, result);
                return uri.ToString();
            }
        }

        public List<ProgramInfo> GetProgramList()
        {
            throw new NotImplementedException();
        }
    }
}