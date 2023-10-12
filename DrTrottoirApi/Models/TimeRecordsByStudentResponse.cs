namespace DrTrottoirApi.Models
{
    public class TimeRecordsByStudentResponse
    {
        public Guid RoundId { get; set; }
        public string RoundName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
