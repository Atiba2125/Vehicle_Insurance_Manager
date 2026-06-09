using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Admin/Vehicles")]
        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles.Include(v => v.Customer).ToListAsync();
            return View(vehicles);
        }

        [Route("Admin/Vehicles/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.Customer)
                .Include(v => v.Policies)
                .FirstOrDefaultAsync(m => m.VehicleId == id);

            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        [Route("Admin/Vehicles/Create")]
        public IActionResult Create()
        {
            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Vehicles/Create")]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.Customers.FindAsync(vehicle.CustomerId);
                if (customer != null)
                {
                    vehicle.OwnerName = customer.CustomerName;
                }
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vehicle record added successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName", vehicle.CustomerId);
            return View(vehicle);
        }

        [Route("Admin/Vehicles/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();

            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName", vehicle.CustomerId);
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Vehicles/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            if (id != vehicle.VehicleId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = await _context.Customers.FindAsync(vehicle.CustomerId);
                    if (customer != null)
                    {
                        vehicle.OwnerName = customer.CustomerName;
                    }
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Vehicle record updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Customers = new SelectList(_context.Customers, "CustomerId", "CustomerName", vehicle.CustomerId);
            return View(vehicle);
        }

        [Route("Admin/Vehicles/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicles
                .Include(v => v.Customer)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Admin/Vehicles/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vehicle record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
