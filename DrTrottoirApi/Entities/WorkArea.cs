using System.ComponentModel.DataAnnotations;

namespace DrTrottoirApi.Entities
{
    public class WorkArea
    {
        public Guid Id { get; set; }

        [StringLength(45)] 
        [Required] 
        public string City { get; set; }
    }
}
