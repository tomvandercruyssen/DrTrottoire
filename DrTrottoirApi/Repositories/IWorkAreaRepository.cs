using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public interface IWorkAreaRepository
    {
        Task<IList<WorkArea>> GetAllWorkAreas();
        Task<WorkArea> GetWorkAreasById(Guid id);
        Task<Guid> CreateWorkArea(CreateWorkAreaRequest request);
        Task DeleteWorkArea(Guid id);
        Task UpdateWorkArea(Guid id, CreateWorkAreaRequest request);
        Task<IList<GeneralUserInfoResponse>> GetUsersByWorkArea(Guid id);
    }
}
