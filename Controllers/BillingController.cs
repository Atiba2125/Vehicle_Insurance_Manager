using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

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

        // ==========================
        // EDIT GET
        // ==========================
        [Route("Admin/Billing/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var billing = await _context.Billings.FindAsync(id);
            if (billing == null) return NotFound();

            return View(billing);
        }

        // ==========================
        // EDIT POST
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Billing/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Billing billing)
        {
            if (id != billing.BillingId) return NotFound();

            // Ignore navigation properties validation
            ModelState.Remove("Customer");
            ModelState.Remove("Policy");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBilling = await _context.Billings.FirstOrDefaultAsync(b => b.BillingId == id);
                    if (existingBilling == null) return NotFound();

                    existingBilling.CustomerName = billing.CustomerName;
                    existingBilling.CustomerPhone = billing.CustomerPhone;
                    existingBilling.CustomerAddProve = billing.CustomerAddProve;
                    existingBilling.VehicleName = billing.VehicleName;
                    existingBilling.VehicleModel = billing.VehicleModel;
                    existingBilling.VehicleRate = billing.VehicleRate;
                    existingBilling.VehicleBodyNumber = billing.VehicleBodyNumber;
                    existingBilling.VehicleEngineNumber = billing.VehicleEngineNumber;
                    existingBilling.BillDate = billing.BillDate;
                    existingBilling.Amount = billing.Amount;
                    existingBilling.PaymentStatus = billing.PaymentStatus;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Billings.Any(e => e.BillingId == billing.BillingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["SuccessMessage"] = "Billing record updated successfully.";
                return RedirectToAction(nameof(Index));
            }

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
