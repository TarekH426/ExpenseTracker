using System.Diagnostics;
using Expense_Tracker.Models;
using ExpenseTracker.BLL.Interfaces;
using ExpenseTracker.DAL.Models.AuthModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IExpenseService _expenseService;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IExpenseService expenseService, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _expenseService = expenseService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Landing page - show features and login/register options
            if (User.Identity!.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["UserName"] = !string.IsNullOrEmpty(user?.FirstName) ? user.FirstName : user?.UserName ?? "User";
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;

            var monthlyTotal = await _expenseService.GetMonthlyTotalAsync(month, year);
            var indexVm = await _expenseService.GetExpenseIndexViewModelAsync(month, year);
            var recent = indexVm.Expenses
                .OrderByDescending(e => e.Date)
                .Take(5)
                .ToList();

            var vm = new HomeViewModel
            {
                MonthlyTotal = monthlyTotal,
                RecentExpenses = recent
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
