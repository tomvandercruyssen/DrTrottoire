namespace DrTrottoirApi.Models
{
    public class GarbageCollectionGarbageTypeResponse
    {
        public DateTime CollectionTime { get; set; }
        public List<string> GarbageTypes { get; set; }
        public bool HasToBeBroughtOutside { get; set; }
    }
}
