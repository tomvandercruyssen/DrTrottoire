namespace DrTrottoirApi.Entities
{
    public class CompanyGarbageCollection
    {
        public Guid Id { get; set; }
        public virtual Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Guid GarbageCollectionId { get; set; }
        public virtual GarbageCollection GarbageCollection { get; set; }
    }
}
