namespace ExpenseTracker.DAL.Models
{
    public class ExpenseIndexViewModel
    {
        public IEnumerable<Expense> Expenses { get; set; } = new List<Expense>();
        public decimal MonthlyTotal { get; set; }
        public Dictionary<string, decimal> CategoryTotals { get; set; } = new Dictionary<string, decimal>();
        public int SelectedMonth { get; set; } = DateTime.Now.Month;
        public int SelectedYear { get; set; } = DateTime.Now.Year;
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}


