using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BillingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Admin/Billing")]
        public async Task<IActionResult> Index()
        {
            var billings = await _context.Billings
                .Include(b => b.Customer)
                .Include(b => b.Policy)
                .ToListAsync();
            return View(billings);
        }

        [Route("Admin/Billing/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var billing = await _context.Billings
                .Include(b => b.Customer)
                .Include(b => b.Policy)
                .FirstOrDefaultAsync(m => m.BillingId == id);

            if (billing == null) return NotFound();

            return View(billing);
        }

        [Route("Admin/Billing/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var billing = await _context.Billings
                .Include(b => b.Customer)
                .Include(b => b.Policy)
                .FirstOrDefaultAsync(m => m.BillingId == id);
            if (billing == null) return NotFound();

            return View(billing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Admin/Billing/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var billing = await _context.Billings.FindAsync(id);
            if (billing != null)
            {
                _context.Billings.Remove(billing);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Billing record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
