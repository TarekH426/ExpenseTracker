using ExpenseTracker.DAL.Models;

namespace Expense_Tracker.Models
{
    public class HomeViewModel
    {
        public decimal MonthlyTotal { get; set; }
        public List<Expense> RecentExpenses { get; set; } = new List<Expense>();
    }
}


