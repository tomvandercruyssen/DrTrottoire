namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class CompanyNotFoundException : Exception
    {
        public CompanyNotFoundException() : base("Company not found") { }

        public CompanyNotFoundException(string name)
            : base("Company with the name " + name + " not found") { }

        public CompanyNotFoundException(string name, Exception innerException)
            : base("Company with the name " + name + " not found", innerException) { }
    }
}
