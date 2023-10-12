namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class InvalidGarbageTypesException : Exception
    {
        public InvalidGarbageTypesException() : base("Invalid garbageTypes") { }
    }
}
