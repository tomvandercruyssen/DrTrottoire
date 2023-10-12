namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class SyndicNotFoundException : Exception
    {
        public SyndicNotFoundException() : base("Syndic not found") { }

        public SyndicNotFoundException(string name)
            : base("Syndic with the name " + name + " not found") { }

        public SyndicNotFoundException(string name, Exception innerException)
            : base("Syndic with the name " + name + " not found", innerException) { }
    }
}
