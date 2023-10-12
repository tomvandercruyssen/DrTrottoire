namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class InvalidRolesException : Exception
    {
        public InvalidRolesException() : base("Invalid roles") { }
    }
}
