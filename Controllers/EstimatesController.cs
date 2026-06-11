using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EstimatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstimatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // INDEX
        // ==========================
        [Route("Admin/Estimates")]
        public async Task<IActionResult> Index()
        {
            var estimates = await _context.Estimates
                .Include(e => e.Customer)
                .OrderByDescending(e => e.DateCreated)
                .ToListAsync();

            return View(estimates);
        }

        // ==========================
        // DETAILS
        // ==========================
        [Route("Admin/Estimates/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var estimate = await _context.Estimates
                .Include(e => e.Customer)
                .FirstOrDefaultAsync(m => m.EstimateId == id);

            if (estimate == null)
                return NotFound();

            return View(estimate);
        }

        // ==========================
        // EDIT GET
        // ==========================
        [Route("Admin/Estimates/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var estimate = await _context.Estimates.FindAsync(id);

            if (estimate == null)
                return NotFound();

            return View(estimate);
        }

        // ==========================
        // EDIT POST
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Estimates/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Estimate estimate)
        {
            if (id != estimate.EstimateId)
                return NotFound();

            var existingEstimate = await _context.Estimates
                .FirstOrDefaultAsync(e => e.EstimateId == id);

            if (existingEstimate == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                existingEstimate.CustomerName = estimate.CustomerName;
                existingEstimate.CustomerPhone = estimate.CustomerPhone;
                existingEstimate.VehicleName = estimate.VehicleName;
                existingEstimate.VehicleModel = estimate.VehicleModel;
                existingEstimate.VehicleRate = estimate.VehicleRate;
                existingEstimate.VehicleWarranty = estimate.VehicleWarranty;
                existingEstimate.VehiclePolicyType = estimate.VehiclePolicyType;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Estimate updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(estimate);
        }

        // ==========================
        // DELETE GET
        // ==========================
        [Route("Admin/Estimates/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var estimate = await _context.Estimates
                .Include(e => e.Customer)
                .FirstOrDefaultAsync(m => m.EstimateId == id);

            if (estimate == null)
                return NotFound();

            return View(estimate);
        }

        // ==========================
        // DELETE POST
        // ==========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Admin/Estimates/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estimate = await _context.Estimates.FindAsync(id);

            if (estimate != null)
            {
                _context.Estimates.Remove(estimate);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Estimate record deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // HELPER METHOD
        // ==========================
        private bool EstimateExists(int id)
        {
            return _context.Estimates.Any(e => e.EstimateId == id);
        }
    }
}