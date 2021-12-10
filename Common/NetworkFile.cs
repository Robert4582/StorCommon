namespace Common
{
    public class NetworkFile
    {
        public Services Service { get; set; }
        public int Timeout { get; set; }

    }
    public class NetworkFile<T> : NetworkFile where T : struct
    {
        T Info { get; set; }

    }
}