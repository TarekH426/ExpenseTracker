using ExpenseTracker.BLL.Interfaces;
using ExpenseTracker.DAL.Data;
using ExpenseTracker.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.BLL.Repositories
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ApplicationDbContext context) : base(context)
        { }
        public async Task<Expense?> GetByIdWithCategoryAsync(int id)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.ExpenseId == id);
        }

        public async Task<IEnumerable<Expense>> GetAllWithCategoryAsync()
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .ToListAsync();
        }

        public async Task<decimal> GetMonthlyTotalAsync(int month, int year)
        {
            return await _context.Expenses
                .Where(e => e.Date.Month == month && e.Date.Year == year)
                .SumAsync(e => e.Amount);
        }

        public async Task<Dictionary<string, decimal>> GetCategoryTotalsAsync(int month, int year)
        {
            return await _context.Expenses
                .Where(e => e.Date.Month == month && e.Date.Year == year)
                .Include(e => e.Category) // Ensure the Category is loaded for grouping
                .GroupBy(e => e.Category.Title)
                .Select(g => new { CategoryTitle = g.Key, Total = g.Sum(x => x.Amount) })
                .ToDictionaryAsync(k => k.CategoryTitle, v => v.Total);
        }
    }
}

