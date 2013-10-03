using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IsEnergyModel.GlobalMetods
{
    public class XmlSerializationHelper
    {
        public static void Serialize<T>(string filename, T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (XmlTextWriter wr = new XmlTextWriter(filename, Encoding.GetEncoding(1251)))
            {
                wr.Formatting = Formatting.Indented;
                xs.Serialize(wr, obj, new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty) }));
            }
        }
        public static byte[] Serialize<T>(T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter wr = new StreamWriter(stream, Encoding.GetEncoding(1251));
                xs.Serialize(wr, obj, new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty) }));
                return Encoding.Default.GetBytes(Encoding.Default.GetString(stream.ToArray()));
            }

        }

        public static T Deserialize<T>(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (StreamReader rd = new StreamReader(filename, Encoding.GetEncoding(1251)))
            {
                return (T)xs.Deserialize(rd);
            }
        }
        public static T Deserialize<T>(Stream file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(file);

        }
        public static T Deserialize<T>(byte[] data)
        {
            using (Stream stream = new MemoryStream(data))
            {
                var xs = new XmlSerializer(typeof(T));
                return (T)xs.Deserialize(stream);
            }
        }

        public static bool CheckTypeDeserialize<T>(byte[] data)
        {
            try
            {
                using (Stream stream = new MemoryStream(data))
                {
                    var xs = new XmlSerializer(typeof(T));
                    XmlReader reader = new XmlTextReader(stream);
                    return xs.CanDeserialize(reader);
                }
            }
            catch { return false; }
        }

    }
}
