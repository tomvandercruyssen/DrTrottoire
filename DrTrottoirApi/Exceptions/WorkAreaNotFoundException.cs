namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class WorkAreaNotFoundException : Exception
    {
        public WorkAreaNotFoundException() : base("WorkArea not found") { }

        public WorkAreaNotFoundException(string name)
            : base("WorkArea with the name " + name + " not found") { }

        public WorkAreaNotFoundException(string name, Exception innerException)
            : base("WorkArea with the name " + name + " not found", innerException) { }
    }
}
