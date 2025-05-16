using System;
using System.Xml.Serialization;

namespace PS5_NOR_Modifier.Common.Helpers
{
    public static class XmlSerializationHelper
    {
        public static T? DeserilazeXmlFromFile<T>(string filePath)
        {
            T? result = default;

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("File path is empty");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Local XML file '" + filePath + "' not found.");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StreamReader reader = new StreamReader(filePath))
            {
                result = (T?)serializer.Deserialize(reader);
            }

            return result;
        }

        public static T? DeserilazeXmlFromString<T>(string xmlData)
        {
            T? result = default;

            if (string.IsNullOrEmpty(xmlData))
            {
                throw new ArgumentNullException("XML data to deserialize is empty");
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader reader = new StringReader(xmlData))
            {
                result = (T?)serializer.Deserialize(reader);
            }

            return result;
        }
    }
}
