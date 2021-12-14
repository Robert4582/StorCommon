namespace Common
{
    public class NetworkFile
    {
        public Services Service { get; set; }
        public int Timeout { get; set; }
        public string CorrelationID { get; set; }

    }
    public class NetworkFile<T> : NetworkFile
    {
        public T Info { get; set; }

    }
}