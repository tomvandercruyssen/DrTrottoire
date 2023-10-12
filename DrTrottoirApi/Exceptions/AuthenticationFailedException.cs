namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException() : base("Authentication failed") { }

        public AuthenticationFailedException(string email)
            : base("Authentication failed for email " + email) { }

        public AuthenticationFailedException(string email, Exception innerException)
            : base("Authentication failed for email " + email, innerException) { }
    }
}
