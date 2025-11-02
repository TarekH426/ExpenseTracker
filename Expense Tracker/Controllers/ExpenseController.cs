using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.BLL.Interfaces;
using ExpenseTracker.DAL.Models;
using ExpenseTracker.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.PL.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExpenseService _expenseService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(
            IUnitOfWork unitOfWork,
            IExpenseService expenseService, 
            ILogger<ExpenseController> logger)
        {
            _unitOfWork = unitOfWork;
            _expenseService = expenseService;
            _logger = logger;
        }

        // GET: Expense
        public async Task<IActionResult> Index(int? month, int? year)
        {
            try
            {
                var viewModel = await _expenseService.GetExpenseIndexViewModelAsync(month, year);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading expenses index");
                TempData["Error"] = "An error occurred while loading expenses.";
                return View(new ExpenseIndexViewModel());
            }
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var expense = await _unitOfWork.Expenses.GetByIdWithCategoryAsync(id.Value);
                if (expense == null)
                {
                    return NotFound();
                }

                return View(expense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading expense details for ID {ExpenseId}", id);
                TempData["Error"] = "An error occurred while loading expense details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Expense/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = await _expenseService.GetCreateExpenseViewModelAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading create expense form");
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel model)
        {
            // Ensure OtherCategory is filled from the correct input if present
            if (Request.Form.ContainsKey("NewCategoryTitle"))
            {
                model.OtherCategory = Request.Form["NewCategoryTitle"];
            }
            try
            {
                if (ModelState.IsValid)
                {
                    await _expenseService.CreateExpenseAsync(model);
                    TempData["Success"] = "Expense created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                // If model is invalid, reload categories for the form
                model = await _expenseService.ReloadExpenseViewModelCategoriesAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating expense");
                TempData["Error"] = "An error occurred while creating the expense.";
                
                // Reload categories for the form
                model = await _expenseService.ReloadExpenseViewModelCategoriesAsync(model);
                return View(model);
            }
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var viewModel = await _expenseService.GetEditExpenseViewModelAsync(id.Value);
                return View(viewModel);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading edit expense form for ID {ExpenseId}", id);
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Expense/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExpenseViewModel model)
        {
            if (Request.Form.ContainsKey("NewCategoryTitle"))
            {
                model.OtherCategory = Request.Form["NewCategoryTitle"];
            }
            if (id != model.ExpenseId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _expenseService.UpdateExpenseAsync(id, model);
                    TempData["Success"] = "Expense updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                // If model is invalid, reload categories for the form
                model = await _expenseService.ReloadExpenseViewModelCategoriesAsync(model);
                return View(model);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating expense with ID {ExpenseId}", id);
                TempData["Error"] = "An error occurred while updating the expense.";
                
                // Reload categories for the form
                model = await _expenseService.ReloadExpenseViewModelCategoriesAsync(model);
                return View(model);
            }
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var expense = await _unitOfWork.Expenses.GetByIdAsync(id.Value);
                if (expense == null)
                {
                    return NotFound();
                }

                return View(expense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading delete expense form for ID {ExpenseId}", id);
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var expense = await _unitOfWork.Expenses.GetByIdAsync(id);
                if (expense != null)
                {
                    _unitOfWork.Expenses.Delete(expense);
                    await _unitOfWork.SaveAsync();
                    TempData["Success"] = "Expense deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Expense not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting expense with ID {ExpenseId}", id);
                TempData["Error"] = "An error occurred while deleting the expense.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Expense/Filter
        [HttpGet]
        public async Task<IActionResult> Filter(int month, int year)
        {
            return RedirectToAction(nameof(Index), new { month, year });
        }
    }
}
