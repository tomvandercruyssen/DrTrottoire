using Microsoft.Build.Framework;

namespace DrTrottoirApi.Entities
{
    public class Picture
    {
        public Guid Id { get; set; }
        [Required]
        public PictureLabel PictureLabel { get; set; }
        [Required]
        public string PictureUrl { get; set; }
        public virtual Guid TaskId { get; set; }
        public virtual Task Task { get; set; }
    }
}
