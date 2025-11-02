using ExpenseTracker.BLL.Interfaces;
using ExpenseTracker.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.BLL.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Categories { get; private set; }
        public IExpenseRepository Expenses { get; private set; }
        public UnitOfWork(ApplicationDbContext context, ICategoryRepository categoryRepository, IExpenseRepository expenseRepository)
        {
            _context = context;
            Categories = categoryRepository;
            Expenses = expenseRepository;
        }
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
