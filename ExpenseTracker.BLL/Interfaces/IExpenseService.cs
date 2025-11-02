using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.DAL.Models;
using ExpenseTracker.DAL.Models;

namespace ExpenseTracker.BLL.Interfaces
{
    public interface IExpenseService
    {
        Task<decimal> GetMonthlyTotalAsync(int month, int year);
        Task<Dictionary<string, decimal>> GetCategoryTotalsAsync(int month, int year);
        
        // Business Logic Operations
        Task<Expense> CreateExpenseAsync(ExpenseViewModel model);
        Task<Expense> UpdateExpenseAsync(int id, ExpenseViewModel model);
        Task<bool> DeleteExpenseAsync(int id);
        
        // View Model Operations
        Task<ExpenseIndexViewModel> GetExpenseIndexViewModelAsync(int? month, int? year);
        Task<ExpenseViewModel> GetCreateExpenseViewModelAsync();
        Task<ExpenseViewModel> GetEditExpenseViewModelAsync(int id);
        Task<ExpenseViewModel> ReloadExpenseViewModelCategoriesAsync(ExpenseViewModel model);
        
        // Category Operations
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
