namespace Common
{
    public class NetworkFile
    {
        public Services Service { get; set; }
        public int Timeout { get; set; }

    }
    public class NetworkFile<T> : NetworkFile
    {
        T Info { get; set; }

    }
}