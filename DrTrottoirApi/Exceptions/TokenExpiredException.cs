namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException() : base("Token is expired") { }
    }
}
