using Microsoft.AspNetCore.Identity;

namespace VehicleShield.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer"; // "Customer" or "Admin"
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
