using System.ComponentModel.DataAnnotations;

namespace VehicleShield.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string CustomerAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
        public ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();
        public ICollection<Billing> Billings { get; set; } = new List<Billing>();
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
