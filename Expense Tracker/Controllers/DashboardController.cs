using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.BLL.Interfaces;
using ExpenseTracker.DAL.Models.AuthModels;
using Microsoft.AspNetCore.Identity;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly ILogger<DashboardController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(IExpenseService expenseService, ILogger<DashboardController> logger, UserManager<AppUser> userManager)
        {
            _expenseService = expenseService;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index(int? month, int? year)
        {
            try
            {
                var selectedMonth = month ?? DateTime.Now.Month;
                var selectedYear = year ?? DateTime.Now.Year;

                var monthlyTotal = await _expenseService.GetMonthlyTotalAsync(selectedMonth, selectedYear);
                var categoryTotals = await _expenseService.GetCategoryTotalsAsync(selectedMonth, selectedYear);

                var topCategories = categoryTotals
                    .OrderByDescending(kv => kv.Value)
                    .Take(3)
                    .Select(kv => new KeyValuePair<string, decimal>(kv.Key, kv.Value))
                    .ToList();

                // Get user information for welcome message
                var user = await _userManager.GetUserAsync(User);
                var userName = !string.IsNullOrEmpty(user?.FirstName) ? user.FirstName : user?.UserName ?? "User";

                var viewModel = new DashboardViewModel
                {
                    SelectedMonth = selectedMonth,
                    SelectedYear = selectedYear,
                    MonthlyTotal = monthlyTotal,
                    TopCategories = topCategories,
                    ChartLabels = topCategories.Select(tc => tc.Key).ToList(),
                    ChartData = topCategories.Select(tc => tc.Value).ToList()
                };

                ViewData["UserName"] = userName;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return View(new DashboardViewModel());
            }
        }
    }
}


