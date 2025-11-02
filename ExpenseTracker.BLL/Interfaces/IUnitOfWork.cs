using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.BLL.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        ICategoryRepository Categories { get; }
        IExpenseRepository Expenses { get; }
        Task<int> SaveAsync();
    }
}
