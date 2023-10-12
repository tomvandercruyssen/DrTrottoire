namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class NoGarbageCollectionsException : Exception
    {
        public NoGarbageCollectionsException() : base("No garbageCollections found") { }
    }
}
