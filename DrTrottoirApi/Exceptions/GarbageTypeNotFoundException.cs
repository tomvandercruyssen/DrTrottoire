namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class GarbageTypeNotFoundException : Exception
    {
        public GarbageTypeNotFoundException() : base("GarbageType not found") { }

        public GarbageTypeNotFoundException(string name)
            : base("GarbageType with the name " + name + " not found") { }

        public GarbageTypeNotFoundException(string name, Exception innerException)
            : base("GarbageType with the name " + name + " not found", innerException) { }
    }
}
