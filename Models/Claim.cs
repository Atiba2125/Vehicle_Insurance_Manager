using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleShield.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        [StringLength(50)]
        public string ClaimNumber { get; set; } = string.Empty; // CLM-XXXXX

        [Required]
        public int PolicyId { get; set; }

        [ForeignKey("PolicyId")]
        public Policy? Policy { get; set; }

        [Required]
        [StringLength(50)]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        public DateTime PolicyStartDate { get; set; }

        [Required]
        public DateTime PolicyEndDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string PlaceOfAccident { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfAccident { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuredAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ClaimableAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

        public DateTime DateFiled { get; set; } = DateTime.Now;
    }
}
