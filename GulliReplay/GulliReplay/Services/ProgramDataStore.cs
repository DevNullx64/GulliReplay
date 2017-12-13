using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[assembly:Xamarin.Forms.Dependency(typeof(GulliReplay.ProgramDataStore))]
namespace GulliReplay
{
    public class ProgramDataStore : IDataStore<ProgramInfo>
    {
        readonly ReplayDataSource DataSource;
        List<ProgramInfo> programs = new List<ProgramInfo>();

        public ProgramDataStore(ReplayDataSource dataSource)
        {
            DataSource = dataSource;
            programs = DataSource.GetProgramList();
        }

        public async Task<bool> AddItemAsync(ProgramInfo item) => await Task.Run(() => false);
        public async Task<bool> UpdateItemAsync(ProgramInfo item) => await Task.Run(() => false);
        public async Task<bool> DeleteItemAsync(string id) => await Task.Run(() => false);

        public async Task<ProgramInfo> GetItemAsync(string id)
        {
            return await Task.FromResult(programs.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<ProgramInfo>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(programs);
        }
    }
}
