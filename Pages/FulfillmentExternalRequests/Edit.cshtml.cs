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
    public class EditModel : PageModel
    {
        private readonly FulfillmentXmlRepository _repo;
        public EditModel(IWebHostEnvironment env)
        {
            _repo = new FulfillmentXmlRepository(env);
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        // Bind the full object for form-based editing
        [BindProperty]
        public FulfillmentExternalRequest Item { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (!id.HasValue) return NotFound();

            var root = _repo.Load();
            if (root?.FulfillmentExternalRequest == null || id.Value < 0 || id.Value >= root.FulfillmentExternalRequest.Count)
                return NotFound();

            Id = id.Value;
            Item = root.FulfillmentExternalRequest[Id];

            // Ensure nested objects / lists are non-null so razor helpers don't throw
            EnsureNested(Item);
            return Page();
        }

     

        public IActionResult OnPost()
        {
            if (Id < 0)
                return BadRequest();

            if (Item == null)
            {
                ModelState.AddModelError(nameof(Item), "Item data is required.");
                return Page();
            }

            // Validate basic model state (you can add more validation attributes if needed)
            //if (!ModelState.IsValid)
            //    return Page();

            var root = _repo.Load();
            if (root?.FulfillmentExternalRequest == null || Id < 0 || Id >= root.FulfillmentExternalRequest.Count)
                return NotFound();

            root.FulfillmentExternalRequest[Id] = Item;
            _repo.SaveAs(root,"tmp");

            return RedirectToPage("./Index");
        }

        private void EnsureNested(FulfillmentExternalRequest item)
        {
            if (item == null) return;

            if (item.PrimaryInfo == null) item.PrimaryInfo = new PrimaryInfo();
            if (item.PrimaryInfo.ISPInformation == null) item.PrimaryInfo.ISPInformation = new ISPInformation();
            if (item.PrimaryInfo.ISPInformation.Address == null) item.PrimaryInfo.ISPInformation.Address = new Address();

            if (item.Vehicles == null) item.Vehicles = new Vehicles();
            if (item.Vehicles.VehicleInfo == null) item.Vehicles.VehicleInfo = new List<VehicleInfo>();

            if (item.Registrants == null) item.Registrants = new Registrants();
            if (item.Registrants.Registrant == null) item.Registrants.Registrant = new List<Registrant>();

            if (item.AdditionalInsureds == null) item.AdditionalInsureds = new AdditionalInsureds();
            if (item.AdditionalInsureds.AdditionalInsured == null) item.AdditionalInsureds.AdditionalInsured = new List<AdditionalInsured>();

            if (item.Terminals == null) item.Terminals = new Terminals();
            if (item.Terminals.TerminalInfo == null) item.Terminals.TerminalInfo = new List<TerminalInfo>();

            if (item.AdditionalInterests == null) item.AdditionalInterests = new AdditionalInterests();
            if (item.AdditionalInterests.AdditionalInterest == null) item.AdditionalInterests.AdditionalInterest = new List<AdditionalInterest>();

            if (item.Policies == null) item.Policies = new Policies();
            if (item.Policies.Policy == null) item.Policies.Policy = new List<Policy>();

            // Ensure nested objects within each policy
            foreach (var policy in item.Policies.Policy)
            {
                if (policy.Coverage == null) policy.Coverage = new List<Coverage>();
                if (policy.InsurePay == null) policy.InsurePay = new InsurePay();

                foreach (var cov in policy.Coverage)
                {   
                    if (cov.NTL == null) cov.NTL = new NTL();
                    if (cov.NTL.Exposures == null) cov.NTL.Exposures = new Exposures();
                    if (cov.NTL.Exposures.Vehicle == null) cov.NTL.Exposures.Vehicle = new List<ExposureVehicle>();

                    if (cov.WorkComp == null) cov.WorkComp = new WorkComp();
                    if (cov.WorkComp.StateExposures == null) cov.WorkComp.StateExposures = new StateExposures();
                    if (cov.WorkComp.StateExposures.State == null) cov.WorkComp.StateExposures.State = new List<State>();
                }
            }
        }
    }
}