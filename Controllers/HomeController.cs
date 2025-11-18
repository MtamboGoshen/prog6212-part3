using System.Diagnostics;
using ContractMonthlyClaim.Models;
using ContractMonthlyClaim.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity; // Required for UserManager
using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaim.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClaimService _claimService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEncryptionService _encryptionService;
        private readonly UserManager<ApplicationUser> _userManager; // Added UserManager

        // Updated constructor to inject UserManager
        public HomeController(
            ILogger<HomeController> logger,
            IClaimService claimService,
            IWebHostEnvironment webHostEnvironment,
            IEncryptionService encryptionService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _claimService = claimService;
            _webHostEnvironment = webHostEnvironment;
            _encryptionService = encryptionService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var claims = await _claimService.GetClaims();
            return View(claims);
        }

        [Authorize(Roles = "Manager, Programme Coordinator")]
        public async Task<IActionResult> ApproverQueue()
        {
            var claims = await _claimService.GetPendingClaims();
            return View(claims);
        }

        // ========== MODIFIED: GET SubmitClaim (Auto-Fill) ==========
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> SubmitClaim()
        {
            // Get current user
            var username = User.Identity?.Name;
            if (username == null) return Unauthorized();

            var user = await _userManager.FindByNameAsync(username);

            // Pre-fill the model with data from the User Profile (set by HR)
            var model = new SubmitClaimViewModel
            {
                LecturerName = $"{user?.FirstName} {user?.LastName}",
                HourlyRate = user?.HourlyRate ?? 0,
                Month = DateTime.Now.ToString("yyyy-MM")
            };

            return View(model);
        }

        // ========== MODIFIED: POST SubmitClaim (Automation & Validation) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> SubmitClaim(SubmitClaimViewModel viewModel)
        {
            // 1. Security: Re-fetch user details to ensure the Rate is correct.
            // This prevents a user from hacking the read-only HTML field.
            var username = User.Identity?.Name;
            var user = await _userManager.FindByNameAsync(username!);

            // Force the values from the database
            viewModel.LecturerName = $"{user?.FirstName} {user?.LastName}";
            viewModel.HourlyRate = user?.HourlyRate ?? 0;

            // 2. Validation: Check the 180-hour limit
            if (viewModel.HoursWorked > 180)
            {
                ModelState.AddModelError("HoursWorked", "Hours worked cannot exceed 180 hours per month.");
            }

            // 3. File Validation Logic
            if (viewModel.Document != null)
            {
                var maxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB
                if (viewModel.Document.Length > maxFileSizeInBytes)
                {
                    ModelState.AddModelError("Document", "The file size cannot exceed 5 MB.");
                }

                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var fileExtension = Path.GetExtension(viewModel.Document.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("Document", "Invalid file type. Only .pdf, .docx, and .xlsx are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                var newClaim = new Claim
                {
                    LecturerName = viewModel.LecturerName,
                    Programme = viewModel.Programme,
                    Month = viewModel.Month,
                    HoursWorked = viewModel.HoursWorked,
                    HourlyRate = viewModel.HourlyRate,
                    Notes = viewModel.Notes,
                    Amount = viewModel.HoursWorked * viewModel.HourlyRate // Auto-calculated total
                };

                if (viewModel.Document != null && viewModel.Document.Length > 0)
                {
                    var encryptedData = await _encryptionService.EncryptFileAsync(viewModel.Document);

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.Document.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await System.IO.File.WriteAllBytesAsync(filePath, encryptedData);
                    newClaim.Filename = uniqueFileName;
                }

                await _claimService.AddClaim(newClaim);
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ViewDocument(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return NotFound();

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", filename);
            if (!System.IO.File.Exists(filePath)) return View("NotFound");

            var encryptedData = await System.IO.File.ReadAllBytesAsync(filePath);
            var decryptedData = await _encryptionService.DecryptFileAsync(encryptedData);

            var contentType = "application/octet-stream";
            var fileExtension = Path.GetExtension(filename).ToLowerInvariant();
            if (fileExtension == ".pdf") contentType = "application/pdf";
            else if (fileExtension == ".docx") contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            else if (fileExtension == ".xlsx") contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(decryptedData, contentType, Path.GetFileName(filename));
        }

        // ... (Approve, Reject, Delete, Details, Edit actions remain unchanged) ...

        [HttpPost]
        [Authorize(Roles = "Manager, Programme Coordinator")]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            await _claimService.UpdateClaimStatus(claimId, "Approved");
            return RedirectToAction("ApproverQueue");
        }

        [HttpPost]
        [Authorize(Roles = "Manager, Programme Coordinator")]
        public async Task<IActionResult> RejectClaim(int claimId)
        {
            await _claimService.UpdateClaimStatus(claimId, "Rejected");
            return RedirectToAction("ApproverQueue");
        }

        [HttpPost]
        [Authorize(Roles = "Manager, Programme Coordinator")]
        public async Task<IActionResult> DeleteClaim(int claimId)
        {
            await _claimService.DeleteClaim(claimId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var claim = await _claimService.GetClaimById(id);
            if (claim == null) return View("NotFound");
            return View(claim);
        }

        [Authorize(Roles = "Manager, Programme Coordinator")]
        public async Task<IActionResult> Edit(int id)
        {
            var claim = await _claimService.GetClaimById(id);
            if (claim == null) return View("NotFound");
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Programme Coordinator")]
        public async Task<IActionResult> Edit(Claim claim)
        {
            if (ModelState.IsValid)
            {
                claim.Amount = claim.HoursWorked * claim.HourlyRate;
                await _claimService.UpdateClaim(claim);
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}