namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class ContextActionFailedException : Exception
    {
        public ContextActionFailedException() { }

        public ContextActionFailedException(string action)
            : base(action + " action failed") { }

        public ContextActionFailedException(string action, Exception innerException)
            : base(action + " action failed", innerException) { }
    }
}
