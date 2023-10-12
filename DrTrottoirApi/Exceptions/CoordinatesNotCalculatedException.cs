namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class CoordinatesNotCalculatedException : Exception
    {
        public CoordinatesNotCalculatedException() : base("Coordinates couldn't be calculated"){ }
    }
}
