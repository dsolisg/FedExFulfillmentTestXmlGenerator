using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace FulfillmentTestXmlGenerator.Pages.FulfillmentExternalRequests
{
    public class IndexModel : PageModel
    {
        private readonly FulfillmentXmlRepository _repo;

        private const string SessionKeyHasVisited = "HasVisitedIndex";

        public IndexModel(IWebHostEnvironment env)
        {
            _repo = new FulfillmentXmlRepository(env);
        }

        public List<FulfillmentExternalRequest> Requests { get; private set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            // Determine first visit using session flag
            var hasVisited = HttpContext.Session.GetString(SessionKeyHasVisited);
            Models.FulfillmentExternalRequests root;
            if (string.IsNullOrEmpty(hasVisited))
            {
                // first time in this session
                root = _repo.LoadBase();
                HttpContext.Session.SetString(SessionKeyHasVisited, "true");
                _repo.SaveAs(root, "tmp");
            }
            else
            {
                root = _repo.Load();
            }

            Requests = root?.FulfillmentExternalRequest ?? new List<FulfillmentExternalRequest>();
        }

        // Add a new empty request and redirect to Edit so the user can populate it
        public IActionResult OnPostAdd()
        {
            var root = _repo.Load() ?? new Models.FulfillmentExternalRequests();
            if (root.FulfillmentExternalRequest == null)
                root.FulfillmentExternalRequest = new List<FulfillmentExternalRequest>();

            var newItem = new FulfillmentExternalRequest
            {
                // initialize common nested collections/objects so edit page doesn't hit nulls
                Vehicles = new Vehicles(),
                Registrants = new Registrants(),
                AdditionalInsureds = new AdditionalInsureds(),
                Terminals = new Terminals(),
                AdditionalInterests = new AdditionalInterests(),
                Policies = new Policies(),
                PrimaryInfo = new PrimaryInfo { ISPInformation = new ISPInformation { Address = new Address() } }
            };

            root.FulfillmentExternalRequest.Add(newItem);
            _repo.SaveAs(root, "tmp");

            var newIndex = root.FulfillmentExternalRequest.Count - 1;
            return RedirectToPage("./Edit", new { id = newIndex });
        }

        // Remove an item by index (id) and reload the index
        public IActionResult OnPostDelete(int id)
        {
            var root = _repo.Load();
            if (root?.FulfillmentExternalRequest == null) return NotFound();

            if (id < 0 || id >= root.FulfillmentExternalRequest.Count)
                return NotFound();

            root.FulfillmentExternalRequest.RemoveAt(id);
            _repo.SaveAs(root,"tmp");

            return RedirectToPage();
        }

        // Create a new empty FulfillmentExternalRequests root (overwrite existing saved file)
        public IActionResult OnPostCreateNew()
        {
            var root = newFulfillmentRequest();
            return RedirectToPage();
        }


        public Models.FulfillmentExternalRequests newFulfillmentRequest()
        {
            var newRoot = new Models.FulfillmentExternalRequests()
            {
                FulfillmentExternalRequest = new List<FulfillmentExternalRequest>()
            };
            var newItem = new FulfillmentExternalRequest
            {
                // initialize common nested collections/objects so edit page doesn't hit nulls
                Vehicles = new Vehicles(),
                Registrants = new Registrants(),
                AdditionalInsureds = new AdditionalInsureds(),
                Terminals = new Terminals(),
                AdditionalInterests = new AdditionalInterests(),
                Policies = new Policies(),
                PrimaryInfo = new PrimaryInfo { ISPInformation = new ISPInformation { Address = new Address() } }
            };

            newRoot.FulfillmentExternalRequest.Add(newItem);

            _repo.SaveAs(newRoot,"tmp");
            StatusMessage = "Created new empty FulfillmentExternalRequests.";
            return newRoot;
        }
        // Export current saved XML as a downloadable file
        // This version accepts an optional CustomLine (bound from the form) and inserts it as an XML comment
        // immediately after the XML declaration (or at the top if no declaration present).
        public IActionResult OnPostExport()
        {
            var root = _repo.Load() ?? new Models.FulfillmentExternalRequests { FulfillmentExternalRequest = new List<FulfillmentExternalRequest>() };

            // Serialize to string
            var xs = new XmlSerializer(typeof(Models.FulfillmentExternalRequests));
            string xml;
            using (var sw = new StringWriter())
            {
                xs.Serialize(sw, root);
                xml = sw.ToString();
            }
            var array = xml.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
            array[1] = "<tns:FulfillmentExternalRequests xmlns:tns=\"http://blcorp.net/PolicyAdministration/FedExUSFulfillment/FulfillmentExternalRequests\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://blcorp.net/PolicyAdministration/FedExUSFulfillment/FulfillmentExternalRequests FulfillmentExternalRequests.xsd\">\"";
            array[^1] = "</tns:FulfillmentExternalRequests>";
            xml = string.Join("\n", array);

            var bytes = Encoding.UTF8.GetBytes(xml);
            return File(bytes, "application/xml", "FulfillmentExternalRequests.xml");
        }

        // Load uploaded XML file and replace/save repository data
        public async Task<IActionResult> OnPostUploadAsync(IFormFile? xmlFile)
        {
            if (xmlFile == null || xmlFile.Length == 0)
            {
                ModelState.AddModelError("xmlFile", "Please select an XML file to upload.");
                return Page();
            }

            try
            {
                var xs = new XmlSerializer(typeof(Models.FulfillmentExternalRequests));
                using var stream = xmlFile.OpenReadStream();
                var streamReader = new StreamReader(stream);
                var array = streamReader.ReadToEnd().Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
                array[2] = "<FulfillmentExternalRequests>";
                array[^2] = "</FulfillmentExternalRequests>";
                var fixedXml = string.Join("\n", array);
                //var obj = xs.Deserialize(fixedXml);
                using (var reader = new StringReader(fixedXml))
                {
                    var obj = xs.Deserialize(reader);

                    if (obj is Models.FulfillmentExternalRequests root)
                    {
                        _repo.SaveAs(root, "tmp");
                        StatusMessage = "Uploaded and saved XML successfully.";
                        return RedirectToPage();
                    }
                }
                ModelState.AddModelError("xmlFile", "Uploaded file does not contain FulfillmentExternalRequests root.");
                return Page();
            }
            catch (XmlException xe)
            {
                ModelState.AddModelError("xmlFile", "Invalid XML: " + xe.Message);
                return Page();
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("xmlFile", "Error processing file: " + ex.Message);
                return Page();
            }
        }
    }
}