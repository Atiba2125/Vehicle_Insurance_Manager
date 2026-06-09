using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Gather statistics for dashboard cards
            ViewBag.TotalCustomers = await _context.Customers.CountAsync();
            ViewBag.TotalVehicles = await _context.Vehicles.CountAsync();
            ViewBag.TotalPolicies = await _context.Policies.CountAsync();
            ViewBag.ActivePoliciesCount = await _context.Policies.CountAsync(p => p.Status == "Active");
            ViewBag.PendingClaimsCount = await _context.Claims.CountAsync(c => c.Status == "Pending");
            ViewBag.TotalEarnings = await _context.Billings.Where(b => b.PaymentStatus == "Paid").SumAsync(b => b.Amount);
            ViewBag.TotalExpenses = await _context.Expenses.SumAsync(e => e.AmountOfExpense);

            // Recent Claims
            var recentClaims = await _context.Claims
                .OrderByDescending(c => c.DateFiled)
                .Take(5)
                .ToListAsync();

            // Recent Billings
            var recentBillings = await _context.Billings
                .OrderByDescending(b => b.BillDate)
                .Take(5)
                .ToListAsync();

            var data = new Tuple<List<Models.Claim>, List<Models.Billing>>(recentClaims, recentBillings);
            return View(data);
        }
    }
}
