using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.DAL.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        public string? Note { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        // Foreign Key to Category
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        // Navigation Property
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
