using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public interface IRoundRepository
    {
        Task<IList<BaseRoundResponse>> GetAllRounds();
        Task<BaseRoundResponse> GetRoundById(Guid id);
        Task<Guid> AddRound(CreateRoundRequest request);
        Task DeleteRound(Guid id);
        Task UpdateRound(Guid id, Round round);
        Task<IList<GetTasksForRoundsResponse>> GetTasksForRound(Guid id);
        Task StartRound(Guid id);
        Task<IList<GetRemarksOfRoundResponse>> GetRemarksOfRound(Guid id);
        Task<IList<Company>> GetCompaniesByRound(Guid roundId);
        Task<int> GetProgressOfRound(Guid id);
    }
}