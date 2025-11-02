using ClosedXML.Excel;
using ExpenseTracker.DAL.Models;
using ExpenseTracker.DAL.Models.AuthModels.DTO_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using XL = ClosedXML.Excel;

namespace ExpenseTracker.BLL.Interfaces
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IExpenseService _expenseService;
        // Add any IUserService if needed for user display

        public ReportController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? month, int? year, string? format)
        {
            int nowMonth = month ?? DateTime.Now.Month;
            int nowYear = year ?? DateTime.Now.Year;

            // Fetch expenses for the month/year
            var expenseIndexVm = await _expenseService.GetExpenseIndexViewModelAsync(nowMonth, nowYear);
            var expenses = expenseIndexVm.Expenses.Select(e => new ReportExpenseDto {
               // UserName = new AuthResponseDto , // TODO: Add logic for real user if available
                Amount = e.Amount,
                Note = e.Note,
                Category = e.Category
            }).ToList();
            var model = new ReportViewModel {
                SelectedMonth = nowMonth,
                SelectedYear = nowYear,
                Expenses = expenses,
                MinYear = DateTime.Now.Year - 5,
                MaxYear = DateTime.Now.Year
            };

            var periodStr = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(nowMonth)}/{nowYear}";

            if (!string.IsNullOrEmpty(format))
            {
                if (format == "pdf")
                {
                    var stream = new MemoryStream();
                    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                    var doc = QuestPDF.Fluent.Document.Create(container => {
                        container.Page(page => {
                            page.Margin(24);
                            page.Size(QuestPDF.Helpers.PageSizes.A4);
                            page.Header().Text($"Expense Report").FontSize(18).Bold();
                            page.Content().Element(c =>
                                c.Table(table => {
                                    table.ColumnsDefinition(columns => {
                                        columns.ConstantColumn(80); // User
                                        columns.ConstantColumn(90); // Period
                                        columns.ConstantColumn(90); // Total
                                        columns.RelativeColumn(); // Category
                                        columns.RelativeColumn(); // Note
                                    });
                                    table.Header(header => {
                                        header.Cell().Text("User").Bold();
                                        header.Cell().Text("Period").Bold();
                                        header.Cell().Text("Total Expense").Bold();
                                        header.Cell().Text("Category").Bold();
                                        header.Cell().Text("Note").Bold();
                                    });
                                    foreach (var exp in expenses) {
                                        table.Cell().Text(exp.UserName ?? "-");
                                        table.Cell().Text(periodStr);
                                        table.Cell().Text(exp.Amount.ToString("C"));
                                        table.Cell().Text(exp.Category?.Title ?? "-");
                                        table.Cell().Text(exp.Note ?? "");
                                    }
                                })
                            );
                            page.Footer()
                                .AlignRight()
                                .Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(8);
                        });
                    });
                    doc.GeneratePdf(stream);
                    stream.Position = 0;
                    return File(stream, "application/pdf", $"ExpenseReport_{periodStr}.pdf");
                }
                if (format == "excel")
                {
                    using var workbook = new XL.XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Report");
                    worksheet.Cell(1, 1).Value = "User";
                    worksheet.Cell(1, 2).Value = "Period";
                    worksheet.Cell(1, 3).Value = "Total Expense";
                    worksheet.Cell(1, 4).Value = "Category";
                    worksheet.Cell(1, 5).Value = "Note";
                    int row = 2;
                    foreach (var exp in expenses)
                    {
                        worksheet.Cell(row, 1).Value = exp.UserName ?? "-";
                        worksheet.Cell(row, 2).Value = periodStr;
                        worksheet.Cell(row, 3).Value = exp.Amount;
                        worksheet.Cell(row, 4).Value = exp.Category?.Title ?? "-";
                        worksheet.Cell(row, 5).Value = exp.Note ?? "";
                        row++;
                    }

                    var range = worksheet.Range(worksheet.FirstCellUsed(), worksheet.LastCellUsed());
                    var table = range.CreateTable();
                    table.Theme = XLTableTheme.TableStyleMedium9;
                    worksheet.Columns().AdjustToContents();
                    using var stream = new MemoryStream();
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ExpenseReport_{periodStr}.xlsx");
                }
            }
            return View(model);
        }
    }
}
