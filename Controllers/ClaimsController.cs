using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;

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

        [Route("Admin/Claims")]
        public async Task<IActionResult> Index()
        {
            var claims = await _context.Claims
                .Include(c => c.Policy)
                .OrderByDescending(c => c.DateFiled)
                .ToListAsync();
            return View(claims);
        }

        [Route("Admin/Claims/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var claim = await _context.Claims
                .Include(c => c.Policy)
                .FirstOrDefaultAsync(m => m.ClaimId == id);

            if (claim == null) return NotFound();

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Claims/Approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            _context.Update(claim);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Claim {claim.ClaimNumber} has been approved successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Claims/Reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            _context.Update(claim);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Claim {claim.ClaimNumber} has been rejected.";
            return RedirectToAction(nameof(Index));
        }

        [Route("Admin/Claims/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var claim = await _context.Claims
                .FirstOrDefaultAsync(m => m.ClaimId == id);
            if (claim == null) return NotFound();

            return View(claim);
        }

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
                TempData["SuccessMessage"] = "Claim record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
