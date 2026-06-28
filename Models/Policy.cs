using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleShield.Models
{
    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }

        [Required]
        [StringLength(50)]
        public string PolicyNumber { get; set; } = string.Empty; // e.g., VS-2024-00892

        [Required]
        public DateTime PolicyDate { get; set; }

        [Required]
        public int PolicyDurationYears { get; set; } = 1;

        [Required]
        public DateTime PolicyEndDate { get; set; }

        [Required]
        [StringLength(50)]
        public string PolicyType { get; set; } = string.Empty; // "Third Party", "Comprehensive", "Premium Elite"

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active"; // "Active", "Expired", "Lapsed"

        // Customer Details
        [Required]
        public int CustomerId { get; set; }
        
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string CustomerAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string CustomerAddProve { get; set; } = string.Empty; // Address Proof text

        // Vehicle Details
        [Required]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VehicleName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VehicleModel { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VehicleVersion { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal VehicleRate { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleWarranty { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string VehicleBodyNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string VehicleEngineNumber { get; set; } = string.Empty;

        public ICollection<Billing> Billings { get; set; } = new List<Billing>();
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();

        public ICollection<Estimate> Estimates { get; set; } = new List<Estimate>();
    }
}
