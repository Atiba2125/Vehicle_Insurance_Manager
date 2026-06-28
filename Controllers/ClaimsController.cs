using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // Claims List
        // =========================
        [Route("Admin/Claims")]
        public async Task<IActionResult> Index()
        {
            var claims = await _context.Claims
                .Include(c => c.Policy)
                .OrderByDescending(c => c.DateFiled)
                .ToListAsync();

            return View(claims);
        }

        // =========================
        // Create Claim (GET)
        // =========================
        [Route("Admin/Claims/Create")]
        public IActionResult Create()
        {
            ViewBag.Policies = new SelectList(
                _context.Policies.OrderByDescending(p => p.PolicyId),
                "PolicyId",
                "PolicyNumber"
            );

            return View();
        }
        // =========================
        // Create Claim (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Claims/Create")]
        public async Task<IActionResult> Create(Claim claim)
        {
            // Auto generated fields ke validation errors remove karo
            ModelState.Remove("ClaimNumber");
            ModelState.Remove("CustomerName");
            ModelState.Remove("PolicyNumber");
            ModelState.Remove("PolicyStartDate");
            ModelState.Remove("PolicyEndDate");
           

            var policy = await _context.Policies
                .FirstOrDefaultAsync(p => p.PolicyId == claim.PolicyId);

            if (policy == null)
            {
                ModelState.AddModelError("", "Invalid Policy Selected");
            }

            if (ModelState.IsValid && policy != null)
            {
                var random = new Random();

                claim.ClaimNumber = $"CLM-{random.Next(10000, 99999)}";

                claim.PolicyNumber = policy.PolicyNumber;
                claim.CustomerName = policy.CustomerName;

                claim.PolicyStartDate = policy.PolicyDate;
                claim.PolicyEndDate = policy.PolicyEndDate;

                claim.InsuredAmount = policy.VehicleRate;

                claim.ClaimableAmount = policy.VehicleRate * 0.50m;

                if (string.IsNullOrEmpty(claim.Status))
                {
                    claim.Status = "Pending";
                }

                claim.DateFiled = DateTime.Now;

                _context.Claims.Add(claim);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim created successfully.";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Policies = new SelectList(
                _context.Policies.OrderByDescending(p => p.PolicyId),
                "PolicyId",
                "PolicyNumber",
                claim.PolicyId
            );

            return View(claim);
        }
        // =========================
        // Details
        // =========================
        [Route("Admin/Claims/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var claim = await _context.Claims
                .Include(c => c.Policy)
                .FirstOrDefaultAsync(m => m.ClaimId == id);

            if (claim == null)
                return NotFound();

            return View(claim);
        }

        // =========================
        // Approve Claim
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Claims/Approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
                return NotFound();

            claim.Status = "Approved";

            _context.Update(claim);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Claim {claim.ClaimNumber} has been approved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Reject Claim
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Claims/Reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
                return NotFound();

            claim.Status = "Rejected";

            _context.Update(claim);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Claim {claim.ClaimNumber} has been rejected.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Delete (GET)
        // =========================
        [Route("Admin/Claims/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var claim = await _context.Claims
                .FirstOrDefaultAsync(m => m.ClaimId == id);

            if (claim == null)
                return NotFound();

            return View(claim);
        }

        // =========================
        // Delete (POST)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Admin/Claims/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim != null)
            {
                _context.Claims.Remove(claim);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Claim record deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}