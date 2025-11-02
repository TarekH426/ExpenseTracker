# 💰 Expense Tracker

A **web-based expense management system** built with **ASP.NET Core MVC**, designed to help users track their daily spending, view reports, and export data to PDF or Excel.  
The project demonstrates solid backend development skills, data handling, and dynamic reporting features.

---

## 🚀 Features

- 🔐 **User Authentication** (Login, JWT Refresh Token)
- 💸 **Add / Edit / Delete Expenses**
- 📊 **Dashboard** with monthly statistics and top categories (Chart.js)
- 📅 **Filter by Month and Year**
- 📥 **Export Reports** to PDF & Excel (QuestPDF + ClosedXML)
- 📁 **Category Management**
- 🌐 **Responsive UI** using Bootstrap

---

## 🧩 Tech Stack

**Backend:** ASP.NET Core MVC (.NET 9)  
**Frontend:** HTML, CSS, Bootstrap, Chart.js  
**Database:** Microsoft SQL Server  
**Libraries:**  
- `ClosedXML` – for Excel export  
- `QuestPDF` – for PDF generation  
- `Entity Framework Core` – for data access  
- `JWT` – for authentication

---

## 🧠 Architecture Overview

ExpenseTracker/
│
├── Controllers/ → Handle user requests (e.g., ExpenseController, ReportController)
├── Models/ → ViewModels and DTOs for data transfer
├── DAL/ → Data Access Layer (EF models, DbContext)
├── BLL/ → Business Logic Layer (services and interfaces)
├── Views/ → Razor views (UI)
└── wwwroot/ → Static files (CSS, JS, images)


## 📈 Dashboard

Displays:
- Total monthly expenses  
- Top 3 expense categories  
- Visual breakdown chart using Chart.js  

🧾 Reports
Users can generate:

PDF Reports using QuestPDF

Excel Reports using ClosedXML

🔐 Authentication
Implemented using JWT tokens and refresh cookies

👨‍💻 Author
Tarek Hesham
Backend Developer (.NET)
📧 thesham426@gmail.com
🌐 www.linkedin.com/in/tarekhesham
