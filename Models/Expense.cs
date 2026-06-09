using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleShield.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        public DateTime DateOfExpense { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeOfExpense { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountOfExpense { get; set; }
    }
}
