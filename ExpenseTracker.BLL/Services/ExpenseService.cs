using ExpenseTracker.BLL.Interfaces;
using ExpenseTracker.DAL.Models;
using ExpenseTracker.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.BLL.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _expenseRepository = expenseRepository ?? throw new ArgumentNullException(nameof(expenseRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<decimal> GetMonthlyTotalAsync(int month, int year)
        {
            return await _expenseRepository.GetMonthlyTotalAsync(month, year);
        }

        public async Task<Dictionary<string, decimal>> GetCategoryTotalsAsync(int month, int year)
        {
            return await _expenseRepository.GetCategoryTotalsAsync(month, year);
        }

        // Business Logic Operations
        public async Task<Expense> CreateExpenseAsync(ExpenseViewModel model)
        {
            int categoryId = model.CategoryId;
            if (categoryId == -1 && !string.IsNullOrWhiteSpace(model.OtherCategory))
            {
                // CategoryId -1 means it's a new custom category
                var category = new Category { Title = model.OtherCategory.Trim() };
                await _categoryRepository.AddAsync(category);
                await _unitOfWork.SaveAsync();
                categoryId = category.CategoryId; // assigned after save
            }

            var expense = new Expense
            {
                Amount = model.Amount,
                Note = model.Note,
                Date = model.Date,
                CategoryId = categoryId
            };

            await _expenseRepository.AddAsync(expense);
            await _unitOfWork.SaveAsync();
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(int id, ExpenseViewModel model)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
            {
                throw new ArgumentException($"Expense with ID {id} not found.");
            }

            expense.Amount = model.Amount;
            expense.Note = model.Note;
            expense.Date = model.Date;
            expense.CategoryId = model.CategoryId;

            _expenseRepository.Update(expense);
            await _unitOfWork.SaveAsync();
            return expense;
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
            {
                return false;
            }

            _expenseRepository.Delete(expense);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // View Model Operations
        public async Task<ExpenseIndexViewModel> GetExpenseIndexViewModelAsync(int? month, int? year)
        {
            var selectedMonth = month ?? DateTime.Now.Month;
            var selectedYear = year ?? DateTime.Now.Year;

            var expenses = await _expenseRepository.GetAllWithCategoryAsync();
            var categories = await _categoryRepository.GetAllAsync();

            // Filter expenses by month and year
            var filteredExpenses = expenses
                .Where(e => e.Date.Month == selectedMonth && e.Date.Year == selectedYear)
                .OrderByDescending(e => e.Date)
                .ToList();

            // Get monthly total and category totals
            var monthlyTotal = await GetMonthlyTotalAsync(selectedMonth, selectedYear);
            var categoryTotals = await GetCategoryTotalsAsync(selectedMonth, selectedYear);

            return new ExpenseIndexViewModel
            {
                Expenses = filteredExpenses,
                MonthlyTotal = monthlyTotal,
                CategoryTotals = categoryTotals,
                SelectedMonth = selectedMonth,
                SelectedYear = selectedYear,
                Categories = categories
            };
        }

        public async Task<ExpenseViewModel> GetCreateExpenseViewModelAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return new ExpenseViewModel
            {
                Categories = categories,
                Date = DateTime.Now
            };
        }

        public async Task<ExpenseViewModel> GetEditExpenseViewModelAsync(int id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
            {
                throw new ArgumentException($"Expense with ID {id} not found.");
            }

            var categories = await _categoryRepository.GetAllAsync();
            return new ExpenseViewModel
            {
                ExpenseId = expense.ExpenseId,
                Amount = expense.Amount,
                Note = expense.Note,
                Date = expense.Date,
                CategoryId = expense.CategoryId,
                Categories = categories,
                CategoryName = expense.Category?.Title
            };
        }

        public async Task<ExpenseViewModel> ReloadExpenseViewModelCategoriesAsync(ExpenseViewModel model)
        {
            var categories = await _categoryRepository.GetAllAsync();
            model.Categories = categories;
            return model;
        }

        // Category Operations
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }
    }
}
