using ExpenseTracker.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.BLL.Interfaces
{
    public interface IExpenseRepository:IGenericRepository<Expense>
    {
        Task<Expense?> GetByIdWithCategoryAsync(int id);
        Task<IEnumerable<Expense>> GetAllWithCategoryAsync();
        Task<decimal> GetMonthlyTotalAsync(int month, int year);
        Task<Dictionary<string, decimal>> GetCategoryTotalsAsync(int month, int year);
    }
}
