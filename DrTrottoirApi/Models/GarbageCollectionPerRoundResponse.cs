namespace DrTrottoirApi.Models
{
    public class GarbageCollectionPerRoundResponse
    {
        public Guid TaskId { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }

        public bool IsFinished { get; set; }
        public bool IsInProgress { get; set; }

       
    }
}
