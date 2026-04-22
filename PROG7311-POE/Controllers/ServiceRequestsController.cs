using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311_POE.Data;
using PROG7311_POE.Models;
using PROG7311_POE.Models.Transport;
using PROG7311_POE.Services;
using System.Diagnostics.Contracts;

namespace PROG7311_POE.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;             //database connection
        private readonly ICurrencyConverter _currencyConverter;     //currency converter

        // constructor, injects the database and currency converter
        public ServiceRequestsController(ApplicationDbContext context, ICurrencyConverter currencyConverter)
        {
            _context = context;
            _currencyConverter = currencyConverter;
        }

        // ServiceRequests - shows contract and client info
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ServiceRequests
                .Include(sr => sr.Contract) // load contract
                .ThenInclude(c => c.Client) // then load client info
                .ToListAsync();             // run query
            return View(requests);          // sends list to index
        }

        //ServiceRequests - shows info for a single service request
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();              // checks the service request ID 
            var request = await _context.ServiceRequests    // finds correct request
                .Include(sr => sr.Contract)                 
                .ThenInclude(c => c.Client)                 
                .FirstOrDefaultAsync(m => m.Id == id);      // return info with matching ID
            if (request == null) return NotFound();         
            return View(request);                           
        }

        //  ServiceRequests Create
        public IActionResult Create()
        {
            // populates dropdown list of active contracts
            ViewBag.ActiveContracts = _context.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == ContractStatus.Active)
                .ToList();
            return View();
        }

        // ServiceRequests Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest model)
        {
            // Validate contract exists and is active
            var contract = await _context.Contracts.FindAsync(model.ContractId);
            if (contract == null)
            {
                ModelState.AddModelError("ContractId", "Invalid contract.");
            }
            else if (contract.Status != ContractStatus.Active)
            {
                ModelState.AddModelError("ContractId",
                    "Service requests can only be created for ACTIVE contracts. This contract is " + contract.Status);
            }

            // transport validation check
            if (ModelState.IsValid)
            {
                // Transportation
                try
                {
                    var transport = TransportFactory.CreateRequest(model.TransportType);
                    // Adjust cost based on transport type 
                    model.Cost = transport.CalculateBaseCost(model.Cost);
                }
                catch (ArgumentException ex)
                {
                    // checks if the mode of transport is valid
                    ModelState.AddModelError("TransportType", ex.Message);
                    ViewBag.ActiveContracts = _context.Contracts
                        .Include(c => c.Client)
                        .Where(c => c.Status == ContractStatus.Active)
                        .ToList();
                    return View(model);
                }

                // Currency Conversion
                model.CostZar = await _currencyConverter.ConvertUsdToZar(model.Cost);

                // Set remaining fields
                model.CreatedAt = DateTime.UtcNow;
                model.Status = RequestStatus.Pending;

                // save to db
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, reload the dropdown
            ViewBag.ActiveContracts = _context.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == ContractStatus.Active)
                .ToList();
            return View(model);
        }

        // ServiceRequests Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            // dropdown of active contracts
            ViewBag.ActiveContracts = _context.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == ContractStatus.Active)
                .ToList();
            return View(request);
        }

        // ServiceRequests Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest model)
        {
            if (id != model.Id) return NotFound();

            // Validate contract status 
            var contract = await _context.Contracts.FindAsync(model.ContractId);
            if (contract == null)
            {
                ModelState.AddModelError("ContractId", "Invalid contract.");
            }
            else if (contract.Status != ContractStatus.Active)
            {
                ModelState.AddModelError("ContractId", "Cannot assign to an inactive contract.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // update db
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // if request was deleted during editing show error
                    if (!ServiceRequestExists(model.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // if validation failed 
            ViewBag.ActiveContracts = _context.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == ContractStatus.Active)
                .ToList();
            return View(model);
        }

        // ServiceRequests Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            // load details with contract and client
            var request = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null) return NotFound();
            return View(request);
        }

        // ServiceRequests Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request != null)
            {
                // delete 
                _context.ServiceRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
            // return to index
            return RedirectToAction(nameof(Index));
        }

        // helper method to check if ID exists
        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }
    }
}