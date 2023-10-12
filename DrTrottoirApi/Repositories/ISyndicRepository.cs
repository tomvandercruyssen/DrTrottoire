using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public interface ISyndicRepository
    {
        Task<Guid> CreateSyndic(CreateSyndicRequest request);
        Task<IList<Syndic>> GetAllSyndics();
        Task<Syndic> GetSyndicById(Guid id);
        Task UpdateSyndic(Guid id, CreateSyndicRequest syndic);
        Task DeleteSyndic(Guid id);
    }
}
