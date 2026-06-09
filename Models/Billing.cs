using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleShield.Models
{
    public class Billing
    {
        [Key]
        public int BillingId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public int PolicyId { get; set; }

        [ForeignKey("PolicyId")]
        public Policy? Policy { get; set; }

        [Required]
        [StringLength(50)]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string CustomerAddProve { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string BillNo { get; set; } = string.Empty; // e.g. BILL-XXXXX

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
        public string VehicleBodyNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string VehicleEngineNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BillDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Paid"; // "Paid", "Pending"
    }
}
