namespace DrTrottoirApi.Models
{
    public class GetGarbageCollectionRequest
    {
        public Guid CompanyId { get; set; } 
        public DateTime Date { get; set; }
    }
}
