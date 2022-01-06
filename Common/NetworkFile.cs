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
        public NetworkFile()
        {
        }
        public NetworkFile(T info)
        {
            Info = info;
        }

        public T Info { get; set; }

    }
}