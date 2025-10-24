using Microsoft.AspNetCore.Hosting;
using Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Services
{
    public class FulfillmentXmlRepository
    {
        private readonly string _dataFolder;

        public FulfillmentXmlRepository(IWebHostEnvironment env)
        {
            _dataFolder = Path.Combine(env.ContentRootPath, "TestData");
        }

        private string GetXmlFilePath()
        {
            if (!Directory.Exists(_dataFolder))
                throw new DirectoryNotFoundException($"TestData folder not found: {_dataFolder}");

            var file = Directory.EnumerateFiles(_dataFolder, "*.xml").FirstOrDefault();
            if (file == null)
                throw new FileNotFoundException("No .xml file found in TestData folder", _dataFolder);

            return file;
        }

        public FulfillmentExternalRequests Load()
        {
            var path = GetXmlFilePath();
            var fulfillmentExternalRequests = DeserializeFromFile<FulfillmentExternalRequests>(path);
            return fulfillmentExternalRequests;
        }

        public void Save(FulfillmentExternalRequests root)
        {
            var path = GetXmlFilePath();
            var xs = new XmlSerializer(typeof(FulfillmentExternalRequests));
            using var writer = new StreamWriter(path, false);
            xs.Serialize(writer, root);
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
    }
}