using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleShield.Models
{
    public class Estimate
    {
        [Key]
        public int EstimateId { get; set; }

        [Required]
        public int CustomerId { get; set; }
        
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VehicleName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VehicleModel { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal VehicleRate { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleWarranty { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string VehiclePolicyType { get; set; } = string.Empty; // "Third Party", "Comprehensive", "Premium Elite"

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
