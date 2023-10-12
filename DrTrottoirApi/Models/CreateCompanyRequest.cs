namespace DrTrottoirApi.Models
{
    public class CreateCompanyRequest
    {
        public string IdKbo { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }

        public string? PictureUrl { get; set; }
        public Guid SyndicId { get; set; }
    }
}
