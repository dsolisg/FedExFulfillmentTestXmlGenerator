using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace FulfillmentTestXmlGenerator.Pages.FulfillmentExternalRequests
{
    public class UpsertModel : PageModel
    {
        private readonly FulfillmentXmlRepository _repo;
        public UpsertModel(IWebHostEnvironment env)
        {
            _repo = new FulfillmentXmlRepository(env);
        }

        // Select an existing item to load (GET)
        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        // Full JSON payload used by the client to edit the entire object
        [BindProperty]
        public string JsonPayload { get; set; }

        // Optional list of all existing items (for selection)
        public List<FulfillmentExternalRequest> AllItems { get; private set; } = new();

        // The currently loaded item (for display/debug)
        public FulfillmentExternalRequest Item { get; private set; } = new();

        public IActionResult OnGet(int? id)
        {
            var root = _repo.Load() ?? new Models.FulfillmentExternalRequests();
            AllItems = root.FulfillmentExternalRequest ?? new List<FulfillmentExternalRequest>();

            if (id.HasValue)
            {
                if (id.Value < 0 || id.Value >= AllItems.Count) return NotFound();
                Id = id.Value;
                Item = AllItems[Id.Value];
            }
            else
            {
                // start a new blank item
                Item = new FulfillmentExternalRequest();
            }

            JsonPayload = JsonSerializer.Serialize(Item, new JsonSerializerOptions { WriteIndented = true });
            return Page();
        }

        public IActionResult OnPostSave()
        {
            if (string.IsNullOrWhiteSpace(JsonPayload))
            {
                ModelState.AddModelError(nameof(JsonPayload), "JSON payload is required.");
                // reload list to re-render page
                var reload = _repo.Load();
                AllItems = reload?.FulfillmentExternalRequest ?? new List<FulfillmentExternalRequest>();
                return Page();
            }

            FulfillmentExternalRequest updated;
            try
            {
                updated = JsonSerializer.Deserialize<FulfillmentExternalRequest>(JsonPayload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (updated == null) throw new JsonException("Deserialized to null");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(nameof(JsonPayload), $"Invalid JSON: {ex.Message}");
                var reload = _repo.Load();
                AllItems = reload?.FulfillmentExternalRequest ?? new List<FulfillmentExternalRequest>();
                return Page();
            }

            var root = _repo.Load() ?? new Models.FulfillmentExternalRequests();
            if (root.FulfillmentExternalRequest == null) root.FulfillmentExternalRequest = new List<FulfillmentExternalRequest>();

            if (Id.HasValue && Id.Value >= 0 && Id.Value < root.FulfillmentExternalRequest.Count)
            {
                // update existing
                root.FulfillmentExternalRequest[Id.Value] = updated;
            }
            else
            {
                // add new
                root.FulfillmentExternalRequest.Add(updated);
                Id = root.FulfillmentExternalRequest.Count - 1;
            }

            _repo.Save(root);

            // Redirect back to the same page with the item's id (so user can continue editing or verify)
            return RedirectToPage("./Upsert", new { id = Id });
        }
    }
}