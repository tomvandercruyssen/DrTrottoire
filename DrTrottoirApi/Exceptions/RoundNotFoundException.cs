namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class RoundNotFoundException : Exception
    {
        public RoundNotFoundException() : base("Round not found") { }

        public RoundNotFoundException(string name)
            : base("Round with the name " + name + " not found") { }

        public RoundNotFoundException(string name, Exception innerException)
            : base("Round with the name " + name + " not found", innerException) { }
    }
}
