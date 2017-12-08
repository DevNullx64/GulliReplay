using GulliReplay.Models;
using GulliReplay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly:Xamarin.Forms.Dependency(typeof(GulliReplay.ProgramDataStore))]
namespace GulliReplay
{
    public class EpisodeDataStore : IDataStore<EpisodeInfo>
    {
        List<EpisodeInfo> episodes;

        public EpisodeDataStore(ProgramInfo program = null)
        {
            if(program != null)
                episodes = program.GetEpisodeList();
            else
                episodes = new List<EpisodeInfo>();
        }

        public async Task<bool> AddItemAsync(EpisodeInfo item) => await Task.Run(() => false);
        public async Task<bool> UpdateItemAsync(EpisodeInfo item) => await Task.Run(() => false);
        public async Task<bool> DeleteItemAsync(string id) => await Task.Run(() => false);

        public async Task<EpisodeInfo> GetItemAsync(string id)
        {
            return await Task.FromResult(episodes.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<EpisodeInfo>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(episodes);
        }
    }
}
