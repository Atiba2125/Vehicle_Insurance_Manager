using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleShield.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Required]
        [StringLength(100)]
        public string VehicleName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; } = string.Empty;

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
        public string BodyNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EngineNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }
        
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}
