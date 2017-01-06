using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server.CommonLib.Util
{
    public static class Converter<T> where T : class
    {
        public static byte[] ObjectToByteArray(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static T ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            T obj = (T)binForm.Deserialize(memStream);
            return obj;
        }

        public static byte[] EncodingUTF8 (string msg)
        {
            return Encoding.UTF8.GetBytes(msg);
        }

        public static string DecodingUTF8String(byte[] msg)
        {
            return Encoding.UTF8.GetString(msg);
        }

        public static byte[] EncodingUnicodeString(string msg)
        {
            return Encoding.Unicode.GetBytes(msg);
        }

        public static string DecodingUnicodeString(byte[] msg)
        {
            return Encoding.Unicode.GetString(msg);
        }


    }
}
