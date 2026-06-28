using System.ComponentModel.DataAnnotations;

namespace VehicleShield.Models
{
    public class InsurancePlan
    {
        public int InsurancePlanId { get; set; }

        [Required]
        public string PlanName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Features { get; set; }

        public bool IsPopular { get; set; }

        public bool IsActive { get; set; }
    }
}
