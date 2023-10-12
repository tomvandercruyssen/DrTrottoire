using DrTrottoirApi.Models;

namespace DrTrottoirApi.Repositories
{
    public interface IGarbageTypeRepository
    {
        Task CreateGarbageType(CreateGarbageTypeRequest request);
        Task<List<GarbageTypeResponse>> GetAllGarbageTypes();
    }
}
