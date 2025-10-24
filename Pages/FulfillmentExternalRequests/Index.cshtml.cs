using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
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

        public void OnGet()
        {
            var root = _repo.Load();
            Requests = root?.FulfillmentExternalRequest ?? new List<FulfillmentExternalRequest>();
        }
    }
}