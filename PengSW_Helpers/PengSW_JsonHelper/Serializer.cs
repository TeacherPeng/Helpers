using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PengSW.JsonHelper
{
    public static class Serializer
    {
        public static byte[] GetJsonBytes(object aObject)
        {
            DataContractJsonSerializer aSerializer = new DataContractJsonSerializer(aObject.GetType());
            using (MemoryStream aStream = new MemoryStream())
            {
                aSerializer.WriteObject(aStream, aObject);
                aStream.Seek(0, SeekOrigin.Begin);
                return aStream.ToArray();
            }
        }

        public static string GetJsonText(object aObject)
        {
            return Encoding.UTF8.GetString(GetJsonBytes(aObject));
        }

        public static T ReadObjectFromJsonFile<T>(string aFileName) where T : class => ReadObjectFromJsonFile<T>(aFileName, Encoding.UTF8);

        public static T ReadObjectFromJsonFile<T>(string aFileName, Encoding aEncoding) where T : class
        {
            string aJsonText = File.ReadAllText(aFileName, aEncoding);
            return ReadObjectFromJsonText<T>(aJsonText);
        }

        public static T ReadObjectFromJsonText<T>(string aJsonText) where T : class
        {
            using (MemoryStream aStream = new MemoryStream(Encoding.UTF8.GetBytes(aJsonText.Trim())))
            {
                DataContractJsonSerializer aJsonSerializer = new DataContractJsonSerializer(typeof(T));
                T aObject = aJsonSerializer.ReadObject(aStream) as T;
                return aObject;
            }
        }
    }
}
