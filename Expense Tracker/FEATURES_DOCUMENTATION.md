# Expense Tracker - Complete Website Features Documentation

## Table of Contents
1. [Overview](#overview)
2. [Authentication & User Management](#authentication--user-management)
3. [Expense Management](#expense-management)
4. [Dashboard & Analytics](#dashboard--analytics)
5. [Reports & Export](#reports--export)
6. [User Interface Features](#user-interface-features)
7. [API Endpoints](#api-endpoints)
8. [Security Features](#security-features)
9. [Default Categories](#default-categories)

---

## Overview

Expense Tracker is a comprehensive web application built with ASP.NET Core MVC that helps users track, manage, and analyze their personal expenses. The application provides a secure, user-friendly interface for managing financial data with features ranging from basic expense tracking to advanced reporting and analytics.

---

## Authentication & User Management

### 1. User Registration
- **Location**: `/Auth/Register`
- **Features**:
  - New user account creation
  - Registration form with validation
  - Required fields:
    - First Name
    - Last Name
    - Username (unique)
    - Email (unique, validated format)
    - Password (strong password requirements)
  - Password requirements:
    - Minimum 6 characters
    - At least one uppercase letter
    - At least one lowercase letter
    - At least one digit
    - At least one special character
  - Automatic role assignment (default: "User")
  - Automatic login after successful registration
  - Redirects to Dashboard after registration

### 2. User Login
- **Location**: `/Auth/Login`
- **Features**:
  - Email-based authentication
  - Password verification
  - Persistent login option (cookies)
  - Support for return URL redirects
  - Error handling for invalid credentials
  - Account lockout protection (5 failed attempts = 5-minute lockout)
  - Redirects to Dashboard after successful login

### 3. User Profile
- **Location**: `/Auth/Profile`
- **Features**:
  - View user profile information
  - Display username
  - Display email address
  - Show account creation date (UTC)
  - Logout functionality
  - Only accessible to authenticated users

### 4. User Logout
- **Features**:
  - Secure session termination
  - Cookie-based logout (GET)
  - Form-based logout (POST) with CSRF protection
  - Redirects to login page after logout
  - Available in navigation menu and profile page

### 5. Welcome Messages
- **Feature**: Personalized welcome messages after login
- **Locations**:
  - Dashboard: Shows "Welcome, [FirstName]!" with current month/year context
  - Home Page: Shows "Welcome back, [FirstName]!" when authenticated
- **Implementation**: Uses user's first name, falls back to username if first name is not available

---

## Expense Management

### 1. View Expenses
- **Location**: `/Expense/Index`
- **Features**:
  - List all expenses in a table format
  - Month and year filtering
  - Display expense details:
    - Date (formatted as yyyy-MM-dd)
    - Category (with icon/emoji support)
    - Amount (currency formatted)
    - Note/description
  - Monthly total summary display
  - Category totals breakdown (sidebar)
  - Actions available:
    - View Details
    - Edit Expense
    - Delete Expense
  - Empty state message when no expenses found
  - Responsive table design

### 2. Create Expense
- **Location**: `/Expense/Create`
- **Features**:
  - Add new expense with form validation
  - Required fields:
    - Amount (must be greater than 0)
    - Date (date picker)
    - Category (dropdown with icons)
    - Note (optional text field)
  - Category selection features:
    - Predefined categories dropdown
    - Visual category icon preview
    - "Other" category option for custom categories
    - Dynamic "Other Category" input field (shown when "Other" is selected)
    - Category icon background display (supports emoji and image URLs)
  - Form validation with error messages
  - Success/error notifications via TempData
  - Redirects to expense list after creation

### 3. Edit Expense
- **Location**: `/Expense/Edit/{id}`
- **Features**:
  - Update existing expense details
  - Pre-populated form with current values
  - Same validation as create form
  - Category icon preview on selection
  - Edit all fields:
    - Amount
    - Date
    - Category
    - Note
  - Success/error notifications
  - Cancel option returns to expense list

### 4. View Expense Details
- **Location**: `/Expense/Details/{id}`
- **Features**:
  - Detailed view of individual expense
  - Displays all expense information:
    - Date
    - Category
    - Amount
    - Note
  - Quick actions:
    - Edit Expense button
    - Back to list button
  - Clean, readable layout

### 5. Delete Expense
- **Location**: `/Expense/Delete/{id}`
- **Features**:
  - Confirmation page before deletion
  - Displays expense details for review
  - Two-step deletion process (confirmation page + POST)
  - Success notification after deletion
  - Automatic redirect to expense list
  - Error handling for not found expenses

### 6. Expense Filtering
- **Features**:
  - Filter by month (1-12)
  - Filter by year (numeric input)
  - Apply filter button
  - Maintains selected filters in URL
  - Updates totals based on filtered period

---

## Dashboard & Analytics

### 1. Dashboard Overview
- **Location**: `/Dashboard/Index`
- **Features**:
  - Welcome message with user's name
  - Current month/year display
  - Monthly expense total (large, prominent display)
  - Top 3 categories visualization
  - Interactive pie chart (Chart.js)
  - Category breakdown list
  - Month/year selector for filtering

### 2. Monthly Total Calculation
- **Feature**: Real-time calculation of total expenses for selected month/year
- **Display**: Large currency-formatted total
- **Updates**: Based on selected month/year filters

### 3. Category Analytics
- **Features**:
  - Top 3 spending categories identification
  - Pie chart visualization:
    - Color-coded segments
    - Legend display
    - Responsive design
    - Chart.js library integration
  - Category totals list
  - Percentage-based breakdown

### 4. Dashboard Filtering
- **Features**:
  - Month selector (1-12)
  - Year selector (numeric)
  - Real-time data update on filter change
  - URL-based filter parameters

### 5. Recent Expenses (Home Dashboard)
- **Location**: `/Home/Dashboard` (alternative dashboard)
- **Features**:
  - Last 5 expenses displayed
  - Ordered by date (most recent first)
  - Quick expense overview

---

## Reports & Export

### 1. Expense Reports
- **Location**: `/Report/Index`
- **Features**:
  - Comprehensive expense report view
  - Month and year filtering
  - Report data includes:
    - Period (Month/Year)
    - Total expenses
    - Category breakdown
    - Individual expense notes
  - Table format with dark theme styling
  - Empty state handling

### 2. PDF Export
- **Feature**: Download expense reports as PDF
- **Implementation**:
  - QuestPDF library integration
  - Professional document format
  - Report includes:
    - Header with "Expense Report" title
    - Table with columns:
      - User
      - Period
      - Total Expense
      - Category
      - Note
    - Footer with generation timestamp
  - File naming: `ExpenseReport_[Month]_[Year].pdf`
  - A4 page size
  - Clean, readable layout

### 3. Excel Export
- **Feature**: Download expense reports as Excel (.xlsx)
- **Implementation**:
  - ClosedXML library integration
  - Excel workbook format
  - Styled table with themes
  - Auto-adjusted column widths
  - Professional table styling (TableStyleMedium9)
  - Report includes same data as PDF
  - File naming: `ExpenseReport_[Month]_[Year].xlsx`

### 4. Report Filtering
- **Features**:
  - Month dropdown selector
  - Year dropdown selector (5 years range: current year ± 5)
  - Filter button
  - Export buttons work with current filters
  - URL parameters for sharing filtered reports

---

## User Interface Features

### 1. Navigation Menu
- **Features**:
  - Responsive navigation bar
  - Brand logo and name
  - Context-aware menu items:
    - **Authenticated Users**:
      - Home
      - Dashboard (with icon)
      - Expenses
      - Reports
      - Profile
      - Logout button
      - Theme toggle button
    - **Unauthenticated Users**:
      - Home
      - Privacy
      - Login button
      - Register button
  - Mobile-responsive hamburger menu
  - Animated navigation indicator (underline animation)
  - Active link highlighting

### 2. Dark/Light Theme Toggle
- **Location**: Navigation bar (for authenticated users)
- **Features**:
  - Toggle between dark and light themes
  - Button icon changes (🌙 for light mode, ☀️ for dark mode)
  - Persistent theme preference (localStorage)
  - System preference detection on first visit
  - Smooth theme transition
  - Applied globally across all pages

### 3. Responsive Design
- **Features**:
  - Bootstrap 5 integration
  - Mobile-first approach
  - Responsive tables
  - Adaptive card layouts
  - Flexible grid system
  - Mobile navigation menu

### 4. Form Validation
- **Features**:
  - Client-side validation (jQuery Validation)
  - Server-side validation (ASP.NET Core)
  - Real-time error messages
  - Validation summary display
  - Field-level error indicators
  - Required field indicators

### 5. Success/Error Notifications
- **Implementation**: TempData messages
- **Features**:
  - Success messages (green)
  - Error messages (red)
  - Auto-dismiss capability
  - Contextual notifications

### 6. Home Page
- **Location**: `/Home/Index`
- **Features**:
  - Landing page with feature overview
  - Three main feature cards:
    1. Add Expense (with receipt icon)
    2. Track on Dashboard (with chart icon)
    3. Export Reports (with document icon)
  - Welcome message for authenticated users
  - Call-to-action buttons:
    - "Get Started" for unauthenticated users
    - "Add Expense" for authenticated users
  - Background image support

### 7. Privacy Page
- **Location**: `/Home/Privacy`
- **Features**: Privacy policy information (available to all users)

---

## API Endpoints

### 1. Authentication API
The application provides REST API endpoints for programmatic access:

#### POST `/api/register`
- **Purpose**: Register new user via API
- **Authentication**: Not required (AllowAnonymous)
- **Request Body**: `RegisterDto` (JSON)
  - Username
  - Email
  - Password
  - FirstName
  - LastName
- **Response**: 
  - `200 OK`: Success with JWT token and refresh token
  - `400 BadRequest`: Validation errors or registration failure
- **Features**:
  - JWT token generation
  - Refresh token in HTTP-only cookie
  - Returns `AuthResponseDto` with token information

#### POST `/api/login`
- **Purpose**: Authenticate user via API
- **Authentication**: Not required (AllowAnonymous)
- **Request Body**: `LoginDto` (JSON)
  - Email
  - Password
- **Response**:
  - `200 OK`: Success with JWT token and refresh token
  - `400 BadRequest`: Invalid credentials
- **Features**:
  - JWT token generation
  - Refresh token in HTTP-only cookie
  - Returns `AuthResponseDto` with token information

### 2. API Documentation
- **Swagger Integration**: Available via Swagger UI
- **Features**:
  - OpenAPI 3.0 specification
  - Interactive API documentation
  - Bearer token authentication scheme
  - Request/response schemas
  - Endpoint descriptions

---

## Security Features

### 1. Authentication & Authorization
- **Identity Framework**: ASP.NET Core Identity
- **Authorization**: Role-based access control
- **Protected Routes**: All expense management routes require authentication
- **Default Role**: "User" role assigned on registration

### 2. Password Security
- **Requirements**:
  - Minimum 6 characters
  - Must contain uppercase letter
  - Must contain lowercase letter
  - Must contain digit
  - Must contain special character
- **Hashing**: Secure password hashing via Identity framework

### 3. Account Security
- **Account Lockout**:
  - 5 failed login attempts = lockout
  - 5-minute lockout duration
  - Automatic unlock after timeout
- **Email Uniqueness**: Enforced at database level
- **Username Uniqueness**: Enforced at database level

### 4. CSRF Protection
- **Anti-Forgery Tokens**: All POST requests protected
- **Token Validation**: Automatic validation on form submissions

### 5. JWT Authentication (API)
- **Token Generation**: Secure JWT tokens for API access
- **Token Validation**: 
  - Issuer validation
  - Audience validation
  - Signature validation
  - Lifetime validation
- **Refresh Tokens**: HTTP-only cookies for token refresh
- **Token Expiration**: Configurable via JWT settings

### 6. Data Protection
- **HTTPS Support**: Secure connection support
- **Cookie Security**: 
  - HttpOnly cookies for refresh tokens
  - Secure flag for production
  - SameSite protection
- **SQL Injection Protection**: Entity Framework parameterized queries

---

## Default Categories

The application seeds the following expense categories on first run:

1. **Food** - Food and dining expenses
2. **Transport** - Transportation costs
3. **Utilities** - Utility bills
4. **Entertainment** - Entertainment expenses
5. **Health** - Healthcare expenses

### Category Features
- Categories can have icons (emoji or image URLs)
- Categories support custom "Other" option
- Categories are user-specific (scoped per user)
- Category totals are calculated dynamically

---

## Technical Stack

### Backend
- **Framework**: ASP.NET Core 9.0 (MVC)
- **Language**: C#
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity + JWT

### Frontend
- **CSS Framework**: Bootstrap 5
- **JavaScript Libraries**:
  - jQuery
  - jQuery Validation
  - Chart.js (for dashboard charts)
- **PDF Generation**: QuestPDF
- **Excel Generation**: ClosedXML

### Architecture
- **Pattern**: MVC (Model-View-Controller)
- **Layers**: 
  - Presentation Layer (PL)
  - Business Logic Layer (BLL)
  - Data Access Layer (DAL)
- **Repository Pattern**: Unit of Work pattern
- **Dependency Injection**: Built-in DI container

---

## User Workflow

### New User Journey
1. Visit Home page
2. Click "Get Started" or navigate to Register
3. Fill registration form
4. Automatically logged in
5. Redirected to Dashboard with welcome message
6. Start adding expenses

### Existing User Journey
1. Visit Home page or navigate to Login
2. Enter credentials
3. Logged in successfully
4. Redirected to Dashboard with personalized welcome
5. Access all features via navigation menu

### Expense Management Workflow
1. Navigate to Expenses page
2. View current month expenses (or filter by month/year)
3. Click "Add Expense" to create new expense
4. Fill form with amount, date, category, and note
5. Submit and redirect to expense list
6. Edit or delete expenses as needed
7. View details for individual expenses

### Reporting Workflow
1. Navigate to Reports page
2. Select desired month and year
3. View expense breakdown in table
4. Download PDF or Excel export
5. Share reports as needed

---

## Additional Notes

### Browser Compatibility
- Modern browsers (Chrome, Firefox, Edge, Safari)
- JavaScript required
- Responsive design for mobile devices

### Data Persistence
- User preferences (theme) stored in localStorage
- All expense data stored in SQL Server database
- Automatic database migrations on startup

### Performance Features
- Responsive UI with Bootstrap
- Optimized database queries
- Efficient data loading
- Chart rendering with Chart.js

---

## Future Enhancements (Noted in Code)
- User name display in reports (currently commented)
- Enhanced category icons support
- Additional export formats (potentially)
- Mobile app integration (via API)

---

**Document Version**: 1.0  
**Last Updated**: Based on current codebase analysis  
**Application**: Expense Tracker  
**Framework**: ASP.NET Core MVC 9.0

