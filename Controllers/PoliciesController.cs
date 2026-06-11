using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PoliciesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PoliciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Admin/Policies")]
        public async Task<IActionResult> Index()
        {
            var policies = await _context.Policies
                .Include(p => p.Customer)
                .Include(p => p.Vehicle)
                .ToListAsync();
            return View(policies);
        }

        [Route("Admin/Policies/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var policy = await _context.Policies
                .Include(p => p.Customer)
                .Include(p => p.Vehicle)
                .Include(p => p.Billings)
                .Include(p => p.Claims)
                .FirstOrDefaultAsync(m => m.PolicyId == id);

            if (policy == null) return NotFound();

            return View(policy);
        }

        [Route("Admin/Policies/Create")]
        public IActionResult Create()
        {
            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            ViewBag.Vehicles = new SelectList(_context.Vehicles, "VehicleId", "VehicleNumber");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Policies/Create")]
        public async Task<IActionResult> Create(Policy policy)
        {
            ModelState.Remove("PolicyNumber");
            ModelState.Remove("PolicyEndDate");
            ModelState.Remove("CustomerName");
            ModelState.Remove("CustomerAddress");
            ModelState.Remove("CustomerPhone");
            ModelState.Remove("CustomerAddProve");
            ModelState.Remove("VehicleNumber");
            ModelState.Remove("VehicleName");
            ModelState.Remove("VehicleModel");
            ModelState.Remove("VehicleVersion");
            ModelState.Remove("VehicleRate");
            ModelState.Remove("VehicleWarranty");
            ModelState.Remove("VehicleBodyNumber");
            ModelState.Remove("VehicleEngineNumber");

            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.FindAsync(policy.CustomerId);
                var vehicle = await _context.Vehicles.FindAsync(policy.VehicleId);

                if (customer != null && vehicle != null)
                {
                    policy.CustomerName = customer.CustomerName;
                    policy.CustomerAddress = customer.CustomerAddress;
                    policy.CustomerPhone = customer.CustomerPhone;

                    policy.VehicleNumber = vehicle.VehicleNumber;
                    policy.VehicleName = vehicle.VehicleName;
                    policy.VehicleModel = vehicle.VehicleModel;
                    policy.VehicleVersion = vehicle.VehicleVersion;
                    policy.VehicleRate = vehicle.VehicleRate;
                    policy.VehicleBodyNumber = vehicle.BodyNumber;
                    policy.VehicleEngineNumber = vehicle.EngineNumber;
                    policy.VehicleWarranty = policy.PolicyDurationYears >= 3 ? "3 Years" : "1 Year";

                    policy.CustomerAddProve = "Address Verified";
                    policy.Status = "Active";
                }

                policy.PolicyEndDate = policy.PolicyDate.AddYears(policy.PolicyDurationYears);

                var random = new Random();
                policy.PolicyNumber = $"VS-{DateTime.Now.Year}-{random.Next(10000, 99999)}";

                _context.Policies.Add(policy);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName", policy.CustomerId);
            ViewBag.Vehicles = new SelectList(_context.Vehicles, "VehicleId", "VehicleNumber", policy.VehicleId);

            return View(policy);
        }

        [Route("Admin/Policies/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var policy = await _context.Policies.FindAsync(id);
            if (policy == null) return NotFound();

            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName", policy.CustomerId);
            ViewBag.Vehicles = new SelectList(_context.Vehicles, "VehicleId", "VehicleNumber", policy.VehicleId);
            return View(policy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Policies/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Policy policy)
        {
            if (id != policy.PolicyId)
                return NotFound();


// Remove validation for auto-filled fields
ModelState.Remove("PolicyNumber");
            ModelState.Remove("PolicyEndDate");
            ModelState.Remove("CustomerName");
            ModelState.Remove("CustomerAddress");
            ModelState.Remove("CustomerPhone");
            ModelState.Remove("CustomerAddProve");
            ModelState.Remove("VehicleNumber");
            ModelState.Remove("VehicleName");
            ModelState.Remove("VehicleModel");
            ModelState.Remove("VehicleVersion");
            ModelState.Remove("VehicleRate");
            ModelState.Remove("VehicleWarranty");
            ModelState.Remove("VehicleBodyNumber");
            ModelState.Remove("VehicleEngineNumber");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPolicy = await _context.Policies
                        .FirstOrDefaultAsync(p => p.PolicyId == id);

                    if (existingPolicy == null)
                        return NotFound();

                    existingPolicy.PolicyDate = policy.PolicyDate;
                    existingPolicy.PolicyDurationYears = policy.PolicyDurationYears;
                    existingPolicy.PolicyType = policy.PolicyType;
                    existingPolicy.Status = policy.Status;

                    existingPolicy.PolicyEndDate =
                        policy.PolicyDate.AddYears(policy.PolicyDurationYears);

                    _context.Update(existingPolicy);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Policy updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.Customers = new SelectList(
                _context.Customers,
                "CustomerId",
                "CustomerName",
                policy.CustomerId);

            ViewBag.Vehicles = new SelectList(
                _context.Vehicles,
                "VehicleId",
                "VehicleNumber",
                policy.VehicleId);

            return View(policy);

}


        [Route("Admin/Policies/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var policy = await _context.Policies
                .Include(p => p.Customer)
                .Include(p => p.Vehicle)
                .FirstOrDefaultAsync(m => m.PolicyId == id);
            if (policy == null) return NotFound();

            return View(policy);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Admin/Policies/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var policy = await _context.Policies.FindAsync(id);
            if (policy != null)
            {
                _context.Policies.Remove(policy);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Policy deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PolicyExists(int id)
        {
            return _context.Policies.Any(e => e.PolicyId == id);
        }
    }
}
