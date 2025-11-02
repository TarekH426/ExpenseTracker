using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.DAL.Models
{
    public class ExpenseViewModel
    {
        public int ExpenseId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Navigation property for dropdown
        public IEnumerable<Category>? Categories { get; set; }

        // For display purposes
        public string? CategoryName { get; set; }

        [Display(Name = "Other Category")]
        public string? OtherCategory { get; set; }

    }
}


