namespace DrTrottoirApi.Models
{
    public class DeleteGarbageCollectionRequest
    {
        public DateTime CollectionTime { get; set; }
        public Guid CompanyId { get; set; }
    }
}
