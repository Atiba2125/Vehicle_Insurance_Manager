using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InsurancePlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InsurancePlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Admin/InsurancePlans")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.InsurancePlans.ToListAsync());
        }

        [Route("Admin/InsurancePlans/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/InsurancePlans/Create")]
        public async Task<IActionResult> Create(InsurancePlan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Insurance Plan added successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        [Route("Admin/InsurancePlans/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.InsurancePlans
                .FirstOrDefaultAsync(x => x.InsurancePlanId == id);

            if (plan == null) return NotFound();

            return View(plan);
        }

        [Route("Admin/InsurancePlans/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.InsurancePlans.FindAsync(id);

            if (plan == null) return NotFound();

            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/InsurancePlans/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, InsurancePlan plan)
        {
            if (id != plan.InsurancePlanId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(plan);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Insurance Plan updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        [Route("Admin/InsurancePlans/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.InsurancePlans
                .FirstOrDefaultAsync(x => x.InsurancePlanId == id);

            if (plan == null) return NotFound();

            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Admin/InsurancePlans/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.InsurancePlans.FindAsync(id);

            if (plan != null)
            {
                _context.InsurancePlans.Remove(plan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}