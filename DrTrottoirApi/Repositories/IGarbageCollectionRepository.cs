using DrTrottoirApi.Models;

namespace DrTrottoirApi.Repositories
{
    public interface IGarbageCollectionRepository
    {
        Task CreateGarbageCollection(CreateGarbageCollectionRequest request);
        Task DeleteGarbageCollection(DeleteGarbageCollectionRequest request);
        Task<BaseGarbageCollectionResponse> GetGarbageCollectionsForWeek(GetGarbageCollectionRequest request);
        Task<IList<GarbageCollectionGarbageTypeResponse>> GetGarbageCollectionsWithGarbageTypesForWeek(GetGarbageCollectionRequest request);
        Task<GarbageCollectionGarbageTypeResponse> GetGarbageCollectionsWithGarbageTypesForTimeSlot(GetGarbageCollectionRequest request);
    }
}
