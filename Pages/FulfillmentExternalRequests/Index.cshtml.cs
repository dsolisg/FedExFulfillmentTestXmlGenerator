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

        public IndexModel(IWebHostEnvironment env)
        {
            _repo = new FulfillmentXmlRepository(env);
        }

        public List<FulfillmentExternalRequest> Requests { get; private set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            var root = _repo.Load(); //new Models.FulfillmentExternalRequests(); // 
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
            //_repo.Save(root);

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
           // _repo.Save(root);

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

            // _repo.Save(newRoot);
            StatusMessage = "Created new empty FulfillmentExternalRequests.";
            return newRoot;
        }
        // Export current saved XML as a downloadable file
        public IActionResult OnPostExport()
        {
            var root = _repo.Load() ?? new Models.FulfillmentExternalRequests { FulfillmentExternalRequest = new List<FulfillmentExternalRequest>() };
            var xs = new XmlSerializer(typeof(Models.FulfillmentExternalRequests));
            using var ms = new MemoryStream();
            using (var writer = new StreamWriter(ms, Encoding.UTF8, 1024, leaveOpen: true))
            {
                xs.Serialize(writer, root);
                writer.Flush();
            }
            ms.Position = 0;
            var bytes = ms.ToArray();
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
                var obj = xs.Deserialize(stream);
                if (obj is Models.FulfillmentExternalRequests root)
                {
                    _repo.Save(root);
                    StatusMessage = "Uploaded and saved XML successfully.";
                    return RedirectToPage();
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