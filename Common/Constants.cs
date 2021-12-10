using System;
using System.Reflection;

namespace Common
{
    public enum Services { PrivateMessage, GroupMessage, GlobalMessage, Exception, Login, Create, Patch, SessionHost, SessionManager }
    public enum RabbitMQExchangeTypes { Direct, Fanout, Headers, Topic }

    public enum QueueInteraction { Listener, Broadcaster, Bidirectional }
    public static class Constants
    {
        public static string Hostname { get; } = "localhost";
    }
    public static class Extensions
    {
        public static string ToLower<T>(this T source) where T : IConvertible//enum
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            return source.ToString().ToLower();
        }
    }
}
