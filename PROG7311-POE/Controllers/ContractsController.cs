using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG7311_POE.Data;
using PROG7311_POE.Models;
using PROG7311_POE.Services;
using PROG7311_POE.ViewModels;

namespace PROG7311_POE.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly INotificationService _notificationService;

        public ContractsController(ApplicationDbContext context, IWebHostEnvironment webHostEnv, INotificationService notificationService)
        {
            _context = context;
            _webHostEnv = webHostEnv;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Client)
                .ToListAsync();
            return View(contracts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        public IActionResult Create()
        {
            ViewBag.Clients = _context.Clients.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.SignedAgreement != null)
                {
                    if (!FileValidator.IsValidPdf(model.SignedAgreement, out string error))
                    {
                        ModelState.AddModelError("SignedAgreement", error);
                        ViewBag.Clients = _context.Clients.ToList();
                        return View(model);
                    }

                    string uploadsFolder = Path.Combine(_webHostEnv.WebRootPath, "uploads", "contracts");
                    Directory.CreateDirectory(uploadsFolder);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SignedAgreement.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.SignedAgreement.CopyToAsync(stream);
                    }
                }

                var contract = new Contract
                {
                    ClientId = model.ClientId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Status = model.Status,
                    ServiceLevel = model.ServiceLevel,
                    SignedAgreementPath = uniqueFileName == null ? "" : $"/uploads/contracts/{uniqueFileName}"
                };

                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = _context.Clients.ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();
            ViewBag.Clients = _context.Clients.ToList();
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile newSignedAgreement)
        {
            if (id != contract.Id) return NotFound();

            var existingContract = await _context.Contracts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (existingContract == null) return NotFound();
            var oldStatus = existingContract.Status;

            if (ModelState.IsValid)
            {
                try
                {
                    if (newSignedAgreement != null)
                    {
                        if (!FileValidator.IsValidPdf(newSignedAgreement, out string error))
                        {
                            ModelState.AddModelError("newSignedAgreement", error);
                            ViewBag.Clients = _context.Clients.ToList();
                            return View(contract);
                        }

                        if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                        {
                            var oldFilePath = Path.Combine(_webHostEnv.WebRootPath, contract.SignedAgreementPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                                System.IO.File.Delete(oldFilePath);
                        }

                        string uploadsFolder = Path.Combine(_webHostEnv.WebRootPath, "uploads", "contracts");
                        Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + newSignedAgreement.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await newSignedAgreement.CopyToAsync(stream);
                        }
                        contract.SignedAgreementPath = $"/uploads/contracts/{uniqueFileName}";
                    }

                    _context.Update(contract);
                    await _context.SaveChangesAsync();

                    if (oldStatus != contract.Status)
                    {
                        _notificationService.NotifyContractStatusChanged(contract, oldStatus, contract.Status);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clients = _context.Clients.ToList();
            return View(contract);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.ServiceRequests)  // Load related service requests
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null) return NotFound();

            // Check if contract has any service requests
            if (contract.ServiceRequests != null && contract.ServiceRequests.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete this contract because it has associated service requests. Please delete the service requests first.";
                return RedirectToAction(nameof(Index));
            }

            // Delete the physical PDF file if it exists
            if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
            {
                var filePath = Path.Combine(_webHostEnv.WebRootPath, contract.SignedAgreementPath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Contract deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var filePath = Path.Combine(_webHostEnv.WebRootPath, contract.SignedAgreementPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/pdf", Path.GetFileName(filePath));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}