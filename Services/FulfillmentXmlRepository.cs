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

        public FulfillmentExternalRequests Load(string? filePath = null)
        {
            var path = filePath ?? Path.Combine(_dataFolder, "tmp.xml");
            var fulfillmentExternalRequests = DeserializeFromFile<FulfillmentExternalRequests>(path);
            return fulfillmentExternalRequests;
        }

        public FulfillmentExternalRequests LoadBase()
        {
            var path = GetXmlFilePath();
            var fulfillmentExternalRequests = DeserializeFromFile<FulfillmentExternalRequests>(path);
            return fulfillmentExternalRequests;
        }

        public void Save(FulfillmentExternalRequests root, string? filePath = null)
        {
            var path = filePath ?? GetXmlFilePath();
            var xs = new XmlSerializer(typeof(FulfillmentExternalRequests));
            using var writer = new StreamWriter(path, false);
            xs.Serialize(writer, root);
        }

        /// <summary>
        /// Save the provided root to a new XML file. If fileName is a full path it will be used directly;
        /// otherwise the file will be created inside the repository TestData folder. If fileName is null or empty
        /// a timestamped filename will be generated. Returns the full path of the saved file.
        /// </summary>
        public string SaveAs(FulfillmentExternalRequests root, string? fileName = null)
        {
            // Ensure TestData folder exists
            if (!Directory.Exists(_dataFolder))
                Directory.CreateDirectory(_dataFolder);

            string fullPath;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var time = System.DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                fileName = $"FulfillmentExternalRequests_{time}.xml";
                fullPath = Path.Combine(_dataFolder, fileName);
            }
            else
            {
                // If fileName looks like a rooted path use it directly, otherwise place it inside TestData
                fullPath = Path.IsPathRooted(fileName) ? fileName : Path.Combine(_dataFolder, fileName);
                // ensure extension
                if (Path.GetExtension(fullPath) == string.Empty)
                {
                    fullPath = fullPath + ".xml";
                }
            }

            var xs = new XmlSerializer(typeof(FulfillmentExternalRequests));
            using var writer = new StreamWriter(fullPath, false, Encoding.UTF8);
            xs.Serialize(writer, root);

            return fullPath;
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