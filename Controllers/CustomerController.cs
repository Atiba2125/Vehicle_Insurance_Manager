using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Data;
using VehicleShield.Models;

namespace VehicleShield.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Get current customer helper
        private async Task<Customer?> GetCurrentCustomerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.CustomerId.HasValue) return null;
            return await _context.Customers
                .Include(c => c.Vehicles)
                .Include(c => c.Policies)
                .Include(c => c.Claims)
                .Include(c => c.Billings)
                .FirstOrDefaultAsync(c => c.CustomerId == user.CustomerId.Value);
        }

        public async Task<IActionResult> Index()
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            ViewBag.ActivePoliciesCount = customer.Policies.Count(p => p.Status == "Active");
            ViewBag.TotalClaimsCount = customer.Claims.Count;
            ViewBag.TotalBillingAmount = customer.Billings.Sum(b => b.Amount);
            ViewBag.ApprovedClaimsAmount = customer.Claims.Where(c => c.Status == "Approved").Sum(c => c.ClaimableAmount);

            return View(customer);
        }

        public async Task<IActionResult> MyPolicies()
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var policies = await _context.Policies
                .Include(p => p.Vehicle)
                .Where(p => p.CustomerId == customer.CustomerId)
                .ToListAsync();

            return View(policies);
        }

        public async Task<IActionResult> PolicyDetails(int id)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var policy = await _context.Policies
                .Include(p => p.Vehicle)
                .Include(p => p.Billings)
                .Include(p => p.Claims)
                .FirstOrDefaultAsync(p => p.PolicyId == id && p.CustomerId == customer.CustomerId);

            if (policy == null) return NotFound();

            return View(policy);
        }

        [HttpGet]
        public async Task<IActionResult> BuyPolicy(string planName = "")
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var activePlans = await _context.InsurancePlans.Where(p => p.IsActive).ToListAsync();
            ViewBag.InsurancePlans = activePlans;
            ViewBag.PreSelectedPlanName = planName;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyPolicy(
            string vehicleName, string vehicleModel, string vehicleVersion, decimal vehicleRate,
            string bodyNumber, string engineNumber, string vehicleNumber,
            string policyType, int durationYears, string customerAddProve)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            // 1. Create and Save Vehicle
            var vehicle = new Vehicle
            {
                VehicleName = vehicleName,
                OwnerName = customer.CustomerName,
                VehicleModel = vehicleModel,
                VehicleVersion = vehicleVersion,
                VehicleRate = vehicleRate,
                BodyNumber = bodyNumber,
                EngineNumber = engineNumber,
                VehicleNumber = vehicleNumber,
                CustomerId = customer.CustomerId
            };
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            // 2. Calculate Premium Rate based on vehicle value and policy type from Database
            var plan = await _context.InsurancePlans.FirstOrDefaultAsync(p => p.IsActive && p.PlanName == policyType);
            decimal annualRate = 0.05m; // fallback 5%
            if (plan != null)
            {
                annualRate = plan.Price / 100m;
            }
            else
            {
                // Fallback for static safety
                if (policyType == "Third Party") annualRate = 0.015m;
                else if (policyType == "Premium Elite") annualRate = 0.08m;
            }

            decimal baseAmount = vehicleRate * annualRate;
            decimal totalAmount = baseAmount * durationYears;

            // 3. Create Policy
            var random = new Random();
            string policyNumber = $"VS-{DateTime.Now.Year}-{random.Next(10000, 99999)}";

            var policy = new Policy
            {
                PolicyNumber = policyNumber,
                PolicyDate = DateTime.Now,
                PolicyDurationYears = durationYears,
                PolicyEndDate = DateTime.Now.AddYears(durationYears),
                PolicyType = policyType,
                Status = "Active",
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                CustomerAddress = customer.CustomerAddress,
                CustomerPhone = customer.CustomerPhone,
                CustomerAddProve = customerAddProve,
                VehicleId = vehicle.VehicleId,
                VehicleNumber = vehicle.VehicleNumber,
                VehicleName = vehicle.VehicleName,
                VehicleModel = vehicle.VehicleModel,
                VehicleVersion = vehicle.VehicleVersion,
                VehicleRate = vehicle.VehicleRate,
                VehicleWarranty = durationYears >= 3 ? "3 Years" : "1 Year",
                VehicleBodyNumber = vehicle.BodyNumber,
                VehicleEngineNumber = vehicle.EngineNumber
            };
            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();

            // 4. Create Billing record (Simulated Payment - marked as Paid directly)
            string billNo = $"BILL-{random.Next(10000, 99999)}";
            var billing = new Billing
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                PolicyId = policy.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                CustomerAddProve = policy.CustomerAddProve,
                CustomerPhone = customer.CustomerPhone,
                BillNo = billNo,
                VehicleName = vehicle.VehicleName,
                VehicleModel = vehicle.VehicleModel,
                VehicleRate = vehicle.VehicleRate,
                VehicleBodyNumber = vehicle.BodyNumber,
                VehicleEngineNumber = vehicle.EngineNumber,
                BillDate = policy.PolicyDate,
                Amount = totalAmount,
                PaymentStatus = "Paid"
            };
            _context.Billings.Add(billing);
            await _context.SaveChangesAsync();

            // 5. Store Estimate for historical reference (Module 3.4 requirement)
            var estimate = new Estimate
            {
                CustomerId = customer.CustomerId,
                PolicyId = policy.PolicyId, // Linked to the newly purchased policy
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhone,
                VehicleName = vehicle.VehicleName,
                VehicleModel = vehicle.VehicleModel,
                VehicleRate = vehicle.VehicleRate,
                VehicleWarranty = policy.VehicleWarranty,
                VehiclePolicyType = policyType,
                DateCreated = policy.PolicyDate
            };
            _context.Estimates.Add(estimate);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Policy purchased successfully! Policy Number: {policyNumber}. Bill Number: {billNo}.";
            return RedirectToAction("MyPolicies");
        }

        [HttpGet]
        public async Task<IActionResult> MyClaims()
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var claims = await _context.Claims
                .Include(c => c.Policy)
                .Where(c => c.Policy!.CustomerId == customer.CustomerId)
                .ToListAsync();

            var activePolicies = await _context.Policies
                .Where(p => p.CustomerId == customer.CustomerId && p.Status == "Active")
                .ToListAsync();

            ViewBag.ActivePolicies = activePolicies;
            return View(claims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitiateClaim(int policyId, string placeOfAccident, DateTime dateOfAccident, decimal claimableAmount)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var policy = await _context.Policies
                .FirstOrDefaultAsync(p => p.PolicyId == policyId && p.CustomerId == customer.CustomerId);

            if (policy == null)
            {
                TempData["ErrorMessage"] = "Invalid Policy selected.";
                return RedirectToAction("MyClaims");
            }

            if (dateOfAccident < policy.PolicyDate || dateOfAccident > policy.PolicyEndDate)
            {
                TempData["ErrorMessage"] = "Date of accident must be within the Policy active period.";
                return RedirectToAction("MyClaims");
            }

            var random = new Random();
            var claim = new Claim
            {
                ClaimNumber = $"CLM-{random.Next(10000, 99999)}",
                PolicyId = policy.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                PolicyStartDate = policy.PolicyDate,
                PolicyEndDate = policy.PolicyEndDate,
                CustomerName = customer.CustomerName,
                PlaceOfAccident = placeOfAccident,
                DateOfAccident = dateOfAccident,
                InsuredAmount = policy.VehicleRate,
                ClaimableAmount = claimableAmount,
                Status = "Pending",
                DateFiled = DateTime.Now
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Claim request {claim.ClaimNumber} submitted successfully! Admin will review it shortly.";
            return RedirectToAction("MyClaims");
        }

        // ==========================================
        // NEW ACTION: Browse Active Insurance Plans
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> BrowsePlans()
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var plans = await _context.InsurancePlans
                .Where(p => p.IsActive)
                .ToListAsync();

            return View(plans);
        }

        // ==========================================
        // NEW ACTION: Unified Invoices / Billings List
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> MyBillings()
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var billings = await _context.Billings
                .Where(b => b.CustomerId == customer.CustomerId)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync();

            return View(billings);
        }

        // ==========================================
        // NEW ACTION: Historical Estimates List
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> MyEstimates()
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            var estimates = await _context.Estimates
                .Where(e => e.CustomerId == customer.CustomerId)
                .OrderByDescending(e => e.DateCreated)
                .ToListAsync();

            return View(estimates);
        }

        // ==========================================
        // NEW ACTION: View Profile Details
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            return View(customer);
        }

        // ==========================================
        // NEW ACTION: Update Profile Details
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(string customerName, string customerAddress, string customerPhone)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerAddress) || string.IsNullOrWhiteSpace(customerPhone))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return View(customer);
            }

            customer.CustomerName = customerName;
            customer.CustomerAddress = customerAddress;
            customer.CustomerPhone = customerPhone;

            _context.Update(customer);
            await _context.SaveChangesAsync();

            // Also update ApplicationUser's FullName to match the Customer Name
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.FullName = customerName;
                await _userManager.UpdateAsync(user);
            }

            TempData["SuccessMessage"] = "Profile details updated successfully!";
            return RedirectToAction("MyProfile");
        }

        // ==========================================
        // NEW ACTION: Submit Feedback / Review
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> SubmitFeedback()
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            // Check if customer already submitted a feedback
            var existing = await _context.CustomerFeedbacks
                .FirstOrDefaultAsync(f => f.CustomerId == customer.CustomerId);

            ViewBag.ExistingFeedback = existing;

            // Pass their active policy types for the tag dropdown
            var myPolicyTypes = await _context.Policies
                .Where(p => p.CustomerId == customer.CustomerId)
                .Select(p => p.PolicyType)
                .Distinct()
                .ToListAsync();

            ViewBag.PolicyTypes = myPolicyTypes;

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(int rating, string reviewText, string policyTag)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account");

            if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(reviewText))
            {
                TempData["ErrorMessage"] = "Rating and review text are required.";
                return RedirectToAction("SubmitFeedback");
            }

            // Check if already submitted — update instead of duplicate
            var existing = await _context.CustomerFeedbacks
                .FirstOrDefaultAsync(f => f.CustomerId == customer.CustomerId);

            if (existing != null)
            {
                existing.Rating = rating;
                existing.ReviewText = reviewText.Trim();
                existing.PolicyTag = string.IsNullOrWhiteSpace(policyTag) ? "General" : policyTag.Trim();
                existing.IsApproved = true; // Re-approval not needed, make it live by default
                existing.SubmittedAt = DateTime.Now;
                _context.Update(existing);
            }
            else
            {
                var feedback = new CustomerFeedback
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    Rating = rating,
                    ReviewText = reviewText.Trim(),
                    PolicyTag = string.IsNullOrWhiteSpace(policyTag) ? "General" : policyTag.Trim(),
                    IsApproved = true, // Live by default on first submit
                    SubmittedAt = DateTime.Now
                };
                _context.CustomerFeedbacks.Add(feedback);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you! Your review has been submitted and is now live on the website.";
            return RedirectToAction("SubmitFeedback");
        }
    }
}
