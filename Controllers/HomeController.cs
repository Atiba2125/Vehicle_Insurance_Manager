using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HappyClients = await _context.Customers.CountAsync();
            ViewBag.ClaimsSettled = await _context.Claims.Where(c => c.Status == "Approved").SumAsync(c => c.ClaimableAmount);
            
            var totalClaims = await _context.Claims.CountAsync();
            var approvedClaims = await _context.Claims.CountAsync(c => c.Status == "Approved");
            
            double successRate = totalClaims == 0 ? 100 : ((double)approvedClaims / totalClaims) * 100;
            ViewBag.ClaimSuccess = Math.Round(successRate, 1);

            // Approved customer feedbacks for testimonials section
            ViewBag.ApprovedFeedbacks = await _context.CustomerFeedbacks
                .Where(f => f.IsApproved)
                .OrderByDescending(f => f.SubmittedAt)
                .Take(12)
                .ToListAsync();

            // Fetch active plans to display dynamically on public website
            var activePlans = await _context.InsurancePlans
                .Where(p => p.IsActive)
                .ToListAsync();

            return View(activePlans);
        }

        [HttpPost]
        public IActionResult SubmitContact(string contactName, string contactPhone, string contactEmail, string contactSubject, string contactMessage)
        {
            // Just simulate receiving contact message
            TempData["SuccessMessage"] = $"Thank you {contactName}! Your message regarding '{contactSubject}' has been received. We will contact you soon.";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
