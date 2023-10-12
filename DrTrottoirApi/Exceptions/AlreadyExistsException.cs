namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException() { }

        public AlreadyExistsException(string objectType)
            : base(objectType + " already exists") { }

        public AlreadyExistsException(string objectType, Exception innerException)
            : base(objectType + " already exists", innerException) { }
    }
}
