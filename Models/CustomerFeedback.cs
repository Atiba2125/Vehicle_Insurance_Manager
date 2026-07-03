using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleShield.Models
{
    public class CustomerFeedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Review must be at least 10 characters.")]
        public string ReviewText { get; set; } = string.Empty;

        // e.g. "Comprehensive Plan", "Third Party", "General"
        [StringLength(100)]
        public string PolicyTag { get; set; } = "General";

        // Admin can approve/reject before it shows publicly
        public bool IsApproved { get; set; } = false;

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
    }
}
