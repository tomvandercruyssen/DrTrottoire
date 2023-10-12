namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("User not found") { }

        public UserNotFoundException(string name)
            : base("User with the name " + name + " not found") { }

        public UserNotFoundException(string name, Exception innerException)
            : base("User with the name " + name + " not found", innerException) { }
    }
}
