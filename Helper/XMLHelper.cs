using System.Text;
using System.Xml.Serialization;

namespace FulfillmentEngineAutomation.Helpers
{
    internal class XMLHelper
    {
        public static string SerializeToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }

        public static T? DeserializeFromXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var stringReader = new StringReader(xml);
            var result = serializer.Deserialize(stringReader);
            return result is T t ? t : default;
        }

        public static void SerializeToFile<T>(T obj, string filePath, Encoding? encoding = null)
        {
            var serializer = new XmlSerializer(typeof(T));
            encoding ??= Encoding.UTF8;
            using var writer = new StreamWriter(filePath, false, encoding);
            serializer.Serialize(writer, obj);
            writer.Close();
        }

        public static void SetXmlRoot(string fileName)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[1] = "<tns:FulfillmentExternalRequests xmlns:tns=\"http://blcorp.net/PolicyAdministration/FedExUSFulfillment/FulfillmentExternalRequests\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://blcorp.net/PolicyAdministration/FedExUSFulfillment/FulfillmentExternalRequests FulfillmentExternalRequests.xsd\">\"";
            arrLine[^1] = "</tns:FulfillmentExternalRequests>";
            File.WriteAllLines(fileName, arrLine);
        }

        public static T? DeserializeFromFile<T>(string filePath, Encoding? encoding = null)
        {
            var serializer = new XmlSerializer(typeof(T));
            encoding ??= Encoding.UTF8;
            using var reader = new StreamReader(filePath, encoding);
            var result = serializer.Deserialize(reader);
            return result is T t ? t : default;
        }

        public static string ReadXmlFile(string filePath, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return File.ReadAllText(filePath, encoding);
        }

        public static T DeserializeXMLFileToObject<T>(string XmlFilename)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(XmlFilename)) return default(T);

            try
            {
                StreamReader xmlStream = new StreamReader(XmlFilename);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                returnObject = (T)serializer.Deserialize(xmlStream);
            }
            catch (Exception ex)
            {
                // ExceptionLogger.WriteExceptionToConsole(ex, DateTime.Now);
                var errorMessage = ex;
            }
            return returnObject;
        }
    }
}