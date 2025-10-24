using System;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace FulfillmentTestXmlGenerator.Pages.FulfillmentExternalRequests
{
    public class EditModel : PageModel
    {
        private readonly FulfillmentXmlRepository _repo;
        public EditModel(IWebHostEnvironment env)
        {
            _repo = new FulfillmentXmlRepository(env);
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        // JSON representation used by the client-side JSON editor
        [BindProperty]
        public string JsonPayload { get; set; }

        public FulfillmentExternalRequest Item { get; private set; }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue) return NotFound();

            var root = _repo.Load();
            if (root?.FulfillmentExternalRequest == null || id.Value < 0 || id.Value >= root.FulfillmentExternalRequest.Count)
                return NotFound();

            Id = id.Value;
            Item = root.FulfillmentExternalRequest[Id];

            // Provide pretty-printed JSON for the client editor
            JsonPayload = JsonSerializer.Serialize(Item, new JsonSerializerOptions { WriteIndented = true });
            
            return Page();
        }

        public IActionResult OnPost()
        {
            if (Id < 0)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(JsonPayload))
            {
                ModelState.AddModelError(nameof(JsonPayload), "JSON payload is required.");
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
                return Page();
            }

            var root = _repo.Load();
            if (root?.FulfillmentExternalRequest == null || Id < 0 || Id >= root.FulfillmentExternalRequest.Count)
                return NotFound();

            root.FulfillmentExternalRequest[Id] = updated;
            _repo.Save(root);

            return RedirectToPage("./Index");
        }
    }
}