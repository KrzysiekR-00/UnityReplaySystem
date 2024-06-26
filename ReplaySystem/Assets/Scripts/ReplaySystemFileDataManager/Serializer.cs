using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace ReplaySystemFileDataManager
{
    internal static class Serializer
    {
        internal static string SerializeToString<T>(this T toSerialize)
        {
            return System.Text.Encoding.UTF8.GetString(Serialize(toSerialize));
        }

        internal static T DeserializeFromString<T>(this string toDeserialize)
        {
            return Deserialize<T>(System.Text.Encoding.UTF8.GetBytes(toDeserialize));
        }

        private static byte[] Serialize<T>(T toSerialize)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using var stream = new MemoryStream();

            var writerSettings = new XmlWriterSettings() { Indent = false };
            using var writer = XmlWriter.Create(stream, writerSettings);

            serializer.WriteObject(writer, toSerialize);
            writer.Flush();
            var result = stream.ToArray();

            return result;

        }

        private static T Deserialize<T>(byte[] byteArray)
        {
            var serializer = new DataContractSerializer(typeof(T));
            using var stream = new MemoryStream(byteArray);

            using var reader = XmlReader.Create(stream);

            var result = (T)serializer.ReadObject(reader);

            return result;
        }
    }
}
