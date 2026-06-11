using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Admin/Customers")]
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers.Include(c => c.User).ToListAsync();
            return View(customers);
        }

        [Route("Admin/Customers/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .Include(c => c.User)
                .Include(c => c.Vehicles)
                .Include(c => c.Policies)
                .Include(c => c.Billings)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        [Route("Admin/Customers/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Customers/Create")]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Customer created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [Route("Admin/Customers/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Customers/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Customers.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.CustomerName = customer.CustomerName;
                    existing.CustomerAddress = customer.CustomerAddress;
                    existing.CustomerPhone = customer.CustomerPhone;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Customer updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [Route("Admin/Customers/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Admin/Customers/Delete/{id}")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer != null)
            {
                // Customer ki vehicles
                var vehicles = _context.Vehicles
                    .Where(v => v.CustomerId == id)
                    .ToList();

                var vehicleIds = vehicles.Select(v => v.VehicleId).ToList();

                // Un vehicles ki policies
                var policies = _context.Policies
                    .Where(p => vehicleIds.Contains(p.VehicleId))
                    .ToList();

                // Customer ki direct policies bhi
                policies.AddRange(
                    _context.Policies
                    .Where(p => p.CustomerId == id)
                    .ToList()
                );

                // Billings
                var billings = _context.Billings
                    .Where(b => b.CustomerId == id)
                    .ToList();

                // Claims
                var policyIds = policies.Select(p => p.PolicyId).ToList();

                var claims = _context.Claims
                    .Where(c => policyIds.Contains(c.PolicyId))
                    .ToList();

                _context.Claims.RemoveRange(claims);
                _context.Billings.RemoveRange(billings);
                _context.Policies.RemoveRange(policies.Distinct());
                _context.Vehicles.RemoveRange(vehicles);

                _context.Customers.Remove(customer);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Customer deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
