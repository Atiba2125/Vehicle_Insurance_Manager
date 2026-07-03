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
            ViewBag.TotalEarnings = await _context.Billings
     .Where(b => b.PaymentStatus == "Paid")
     .Select(b => (decimal?)b.Amount)
     .SumAsync() ?? 0;

            ViewBag.TotalExpenses = await _context.Expenses
                .Select(e => (decimal?)e.AmountOfExpense)
                .SumAsync() ?? 0;
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

            ViewBag.PendingFeedbacksCount = await _context.CustomerFeedbacks.CountAsync(f => !f.IsApproved);

            var data = new Tuple<List<Models.Claim>, List<Models.Billing>>(recentClaims, recentBillings);
            return View(data);
        }

        // ==========================================
        // Manage Customer Feedbacks
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> ManageFeedbacks()
        {
            var feedbacks = await _context.CustomerFeedbacks
                .OrderByDescending(f => f.SubmittedAt)
                .ToListAsync();

            return View(feedbacks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveFeedback(int id)
        {
            var fb = await _context.CustomerFeedbacks.FindAsync(id);
            if (fb == null) return NotFound();
            fb.IsApproved = true;
            _context.Update(fb);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{fb.CustomerName} ki review approve kar di gayi!";
            return RedirectToAction("ManageFeedbacks");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectFeedback(int id)
        {
            var fb = await _context.CustomerFeedbacks.FindAsync(id);
            if (fb == null) return NotFound();
            fb.IsApproved = false;
            _context.Update(fb);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{fb.CustomerName} ki review reject/unpublish kar di gayi.";
            return RedirectToAction("ManageFeedbacks");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var fb = await _context.CustomerFeedbacks.FindAsync(id);
            if (fb != null)
            {
                _context.CustomerFeedbacks.Remove(fb);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Review delete kar di gayi.";
            }
            return RedirectToAction("ManageFeedbacks");
        }
    }
}
