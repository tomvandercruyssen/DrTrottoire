namespace DrTrottoirApi.Entities
{
    public class GarbageCollectionGarbageType
    {
        public Guid Id { get; set; }
        public virtual Guid GarbageCollectionId { get; set; } 
        public virtual int GarbageTypeId { get; set; } 
        public virtual GarbageCollection GarbageCollection { get; set; }
        public virtual GarbageType GarbageType { get; set; }
    }
}
