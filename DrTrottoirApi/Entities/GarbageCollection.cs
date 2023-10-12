using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Entities
{
    public class GarbageCollection
    {
        public Guid Id { get; set; }
        public DateTime CollectionTime { get; set; }
        public bool HasToBeBroughtOutside { get; set; }
    }
}
