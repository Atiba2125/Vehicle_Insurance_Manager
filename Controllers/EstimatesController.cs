using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;

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

        [Route("Admin/Estimates")]
        public async Task<IActionResult> Index()
        {
            var estimates = await _context.Estimates
                .Include(e => e.Customer)
                .OrderByDescending(e => e.DateCreated)
                .ToListAsync();
            return View(estimates);
        }

        [Route("Admin/Estimates/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var estimate = await _context.Estimates
                .Include(e => e.Customer)
                .FirstOrDefaultAsync(m => m.EstimateId == id);
            if (estimate == null) return NotFound();

            return View(estimate);
        }

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
                TempData["SuccessMessage"] = "Estimate record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
