using System.Text.Json.Serialization;

namespace Expense_Tracker.Models
{
    public class DashboardViewModel
    {
        public int SelectedMonth { get; set; } = DateTime.Now.Month;
        public int SelectedYear { get; set; } = DateTime.Now.Year;

        public decimal MonthlyTotal { get; set; }

        // Top categories limited to 3
        public List<KeyValuePair<string, decimal>> TopCategories { get; set; } = new List<KeyValuePair<string, decimal>>();

        // Chart data
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<decimal> ChartData { get; set; } = new List<decimal>();
    }
}


