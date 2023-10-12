namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class InvalidKboException : Exception
    {
        public InvalidKboException() : base("Invalid KBO number") { }
    }
}
