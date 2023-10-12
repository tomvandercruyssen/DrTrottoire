namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class TokenNotFoundException : Exception
    {
        public TokenNotFoundException() : base("Token not found") { }
    }
}
