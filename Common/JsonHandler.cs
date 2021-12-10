using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common
{
    public static class Json
    {
        public static string SerializeToString<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }
        public static byte[] SerializeToBytes<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        public static T DeserializeFromString<T>(string objString)
        {
            return (T)JsonSerializer.Deserialize(objString, typeof(T));
        }
        public static T DeserializeFromBytes<T>(byte[] objBytes)
        {
            return (T)JsonSerializer.Deserialize(objBytes, typeof(T));
        }
    }
}
