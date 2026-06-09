using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Admin/Expenses")]
        public async Task<IActionResult> Index()
        {
            var expenses = await _context.Expenses
                .OrderByDescending(e => e.DateOfExpense)
                .ToListAsync();
            return View(expenses);
        }

        [Route("Admin/Expenses/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(m => m.ExpenseId == id);

            if (expense == null) return NotFound();

            return View(expense);
        }

        [Route("Admin/Expenses/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Expenses/Create")]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Expense record added successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        [Route("Admin/Expenses/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Expenses/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.ExpenseId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Expense record updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.ExpenseId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        [Route("Admin/Expenses/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(m => m.ExpenseId == id);
            if (expense == null) return NotFound();

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Admin/Expenses/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Expense record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.ExpenseId == id);
        }
    }
}
