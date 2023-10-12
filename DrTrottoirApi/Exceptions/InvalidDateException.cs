namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class InvalidDateException : Exception
    {
        public InvalidDateException() : base("Date not correct") { }
    }
}
