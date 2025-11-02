using System;
using System.Collections.Generic;

namespace ExpenseTracker.DAL.Models
{
    public class ReportViewModel
    {
        public int SelectedMonth { get; set; } = DateTime.Now.Month;
        public int SelectedYear { get; set; } = DateTime.Now.Year;
        public int MinYear { get; set; } = DateTime.Now.Year - 5;
        public int MaxYear { get; set; } = DateTime.Now.Year;
        public List<ReportExpenseDto> Expenses { get; set; } = new List<ReportExpenseDto>();
    }

    public class ReportExpenseDto
    {
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public Category? Category { get; set; }
    }
}
