using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VehicleShield.Data;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Admin/Reports")]
        public async Task<IActionResult> Index()
        {
            // Monthly Sales Report Data
            var salesReport = await _context.Policies
                .Include(p => p.Billings)
                .GroupBy(p => new { Year = p.PolicyDate.Year, Month = p.PolicyDate.Month })
                .Select(g => new MonthlySalesReportItem
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    PolicyCount = g.Count(),
                    TotalRevenue = g.SelectMany(p => p.Billings).Sum(b => b.Amount)
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToListAsync();

            // Vehicle Wise Analysis Report Data
            var vehicleReport = await _context.Policies
                .GroupBy(p => p.VehicleName)
                .Select(g => new VehicleWiseReportItem
                {
                    VehicleName = g.Key,
                    PolicyCount = g.Count(),
                    TotalInsuredAmount = g.Sum(p => p.VehicleRate)
                })
                .OrderByDescending(x => x.PolicyCount)
                .ToListAsync();

            // Claims Report Data
            var claims = await _context.Claims.ToListAsync();
            var claimsReport = new ClaimsReportSummary
            {
                TotalClaims = claims.Count,
                ApprovedClaims = claims.Count(c => c.Status == "Approved"),
                PendingClaims = claims.Count(c => c.Status == "Pending"),
                RejectedClaims = claims.Count(c => c.Status == "Rejected"),
                TotalClaimableAmount = claims.Sum(c => c.ClaimableAmount),
                TotalApprovedAmount = claims.Where(c => c.Status == "Approved").Sum(c => c.ClaimableAmount)
            };

            // Policies Due Renewals Report (Expires in next 30 days)
            var today = DateTime.Today;
            var thirtyDaysFromNow = today.AddDays(30);
            var dueRenewals = await _context.Policies
                .Include(p => p.Customer)
                .Where(p => p.PolicyEndDate >= today && p.PolicyEndDate <= thirtyDaysFromNow && p.Status == "Active")
                .ToListAsync();

            // Lapsed Policies Report (End date in the past, or marked as Lapsed/Expired)
            var lapsedPolicies = await _context.Policies
                .Include(p => p.Customer)
                .Where(p => p.PolicyEndDate < today || p.Status == "Lapsed")
                .ToListAsync();

            var viewModel = new ReportsViewModel
            {
                MonthlySales = salesReport,
                VehicleWise = vehicleReport,
                ClaimsSummary = claimsReport,
                ClaimsList = claims,
                DueRenewals = dueRenewals,
                LapsedPolicies = lapsedPolicies
            };

            return View(viewModel);
        }
    }

    public class ReportsViewModel
    {
        public List<MonthlySalesReportItem> MonthlySales { get; set; } = new();
        public List<VehicleWiseReportItem> VehicleWise { get; set; } = new();
        public ClaimsReportSummary ClaimsSummary { get; set; } = new();
        public List<Models.Claim> ClaimsList { get; set; } = new();
        public List<Models.Policy> DueRenewals { get; set; } = new();
        public List<Models.Policy> LapsedPolicies { get; set; } = new();
    }

    public class MonthlySalesReportItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int PolicyCount { get; set; }
        public decimal TotalRevenue { get; set; }

        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");
    }

    public class VehicleWiseReportItem
    {
        public string VehicleName { get; set; } = string.Empty;
        public int PolicyCount { get; set; }
        public decimal TotalInsuredAmount { get; set; }
    }

    public class ClaimsReportSummary
    {
        public int TotalClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int PendingClaims { get; set; }
        public int RejectedClaims { get; set; }
        public decimal TotalClaimableAmount { get; set; }
        public decimal TotalApprovedAmount { get; set; }
    }
}
