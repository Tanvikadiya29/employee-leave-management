# Employee Leave Management System

A full-stack web application for managing employee leave requests — built with **.NET 8** (backend REST API), **PostgreSQL** (database), and **React 19 + Tailwind CSS v4** (frontend). The system supports two roles — **Admin** and **Employee** — with complete leave lifecycle management, JWT-based authentication, and a responsive UI.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Tech Stack](#tech-stack)
3. [Folder Structure](#folder-structure)
4. [Database Schema](#database-schema)
5. [Backend Architecture](#backend-architecture)
6. [API Endpoints Reference](#api-endpoints-reference)
7. [Frontend Architecture](#frontend-architecture)
8. [Role-Based Features](#role-based-features)
9. [Authentication Flow](#authentication-flow)
10. [Getting Started](#getting-started)
11. [Environment Configuration](#environment-configuration)
12. [Running Tests](#running-tests)
13. [Default Credentials](#default-credentials)

---

## Project Overview

The Employee Leave Management System allows organizations to digitize and streamline the leave approval process. Employees submit leave requests through a self-service portal; admins review and approve or reject them. The system enforces business rules such as preventing overlapping leaves, disallowing past-dated requests, and maintaining full audit history.

**Core capabilities:**
- Secure login with JWT tokens (role-aware, 8-hour sessions)
- Admin: register employees, manage profiles, approve/reject leaves, view statistics
- Employee: submit leave requests, view personal history, cancel pending requests
- Paginated data tables, status filtering, modal-based forms
- Soft-delete for employee records (data preserved, access disabled)
- Full xUnit test suite for all service-layer business logic

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core 8 (C#) |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL |
| Authentication | JWT Bearer Tokens (BCrypt password hashing) |
| API Docs | Swagger / OpenAPI |
| Frontend | React 19, Vite 8 |
| Styling | Tailwind CSS v4 |
| HTTP Client | Axios |
| Forms | React Hook Form + Yup |
| Routing | React Router v7 |
| Notifications | React Toastify |
| Testing | xUnit, Moq, FluentAssertions, EF In-Memory |

---

## Folder Structure

```
employee-leave-management/
│
├── Backend/
│   └── LeaveManagement/
│       ├── LeaveManagement.API/                  # Main ASP.NET Core Web API project
│       │   ├── Controllers/
│       │   │   ├── AuthController.cs             # Login and user registration
│       │   │   ├── EmployeesController.cs        # Employee CRUD operations
│       │   │   └── LeaveRequestsController.cs    # Leave request lifecycle management
│       │   │
│       │   ├── Models/                           # EF Core entity models
│       │   │   ├── User.cs                       # Employee / user entity
│       │   │   ├── Role.cs                       # Admin / Employee role entity
│       │   │   └── LeaveRequest.cs               # Leave request entity + LeaveStatus enum
│       │   │
│       │   ├── DTOs/                             # Data Transfer Objects
│       │   │   ├── Auth/
│       │   │   │   ├── LoginDto.cs               # email + password
│       │   │   │   ├── RegisterDto.cs            # New user registration payload
│       │   │   │   └── AuthResponseDto.cs        # JWT + user metadata response
│       │   │   ├── Employee/
│       │   │   │   ├── CreateEmployeeDto.cs      # Create employee payload
│       │   │   │   ├── UpdateEmployeeDto.cs      # Partial update payload
│       │   │   │   └── EmployeeResponseDto.cs    # Employee data returned to client
│       │   │   ├── Leave/
│       │   │   │   ├── CreateLeaveRequestDto.cs  # Leave submission payload
│       │   │   │   ├── LeaveResponseDto.cs       # Leave data returned to client
│       │   │   │   └── ReviewLeaveDto.cs         # Approve/reject payload (optional remarks)
│       │   │   └── Common/
│       │   │       ├── ApiResponse.cs            # Generic API response wrapper T
│       │   │       └── PaginationParams.cs       # Page + page size parameters
│       │   │
│       │   ├── Services/
│       │   │   ├── Interfaces/
│       │   │   │   ├── IAuthService.cs
│       │   │   │   ├── IEmployeeService.cs
│       │   │   │   └── ILeaveRequestService.cs
│       │   │   ├── AuthService.cs                # Login + register business logic
│       │   │   ├── EmployeeService.cs            # Employee CRUD business logic
│       │   │   ├── LeaveRequestService.cs        # Leave lifecycle business logic
│       │   │   └── JwtService.cs                 # JWT token generation
│       │   │
│       │   ├── Data/
│       │   │   └── AppDbContext.cs               # EF Core DbContext with Fluent API config
│       │   │
│       │   ├── Middleware/
│       │   │   └── ExceptionMiddleware.cs        # Global unhandled exception handler
│       │   │
│       │   ├── Migrations/                       # EF Core auto-generated migrations
│       │   ├── Properties/
│       │   │   └── launchSettings.json           # Dev launch profiles (HTTP / HTTPS)
│       │   ├── Program.cs                        # App entry point, DI, middleware pipeline
│       │   ├── appsettings.json                  # DB connection string + JWT config
│       │   ├── appsettings.Development.json      # Dev overrides
│       │   └── LeaveManagement.API.csproj
│       │
│       └── LeaveManagement.Tests/               # xUnit test project
│           ├── Services/
│           │   ├── AuthServiceTests.cs
│           │   ├── EmployeeServiceTests.cs
│           │   └── LeaveRequestServiceTests.cs
│           ├── TestHelpers/
│           │   ├── DbContextFactory.cs           # In-memory EF context factory
│           │   ├── JwtConfigFactory.cs           # Test JWT config builder
│           │   └── SeedData.cs                   # Shared test seed data
│           └── LeaveManagement.Tests.csproj
│
├── Frontend/
│   └── leave-management-ui/                     # React + Vite SPA
│       ├── src/
│       │   ├── api/
│       │   │   ├── axiosInstance.js              # Axios base config + JWT interceptor
│       │   │   ├── authApi.js                    # Login / register API calls
│       │   │   ├── employeeApi.js                # Employee CRUD API calls
│       │   │   └── leaveApi.js                   # Leave request API calls
│       │   │
│       │   ├── components/
│       │   │   ├── common/
│       │   │   │   ├── Navbar.jsx                # Top bar: user info + logout
│       │   │   │   ├── Sidebar.jsx               # Navigation links (role-aware)
│       │   │   │   ├── ConfirmModal.jsx          # Reusable yes/no confirmation dialog
│       │   │   │   ├── LoadingSpinner.jsx        # Full-screen async loader
│       │   │   │   ├── Pagination.jsx            # Page controls with item count
│       │   │   │   └── StatusBadge.jsx           # Color-coded leave status pill
│       │   │   ├── employee/
│       │   │   │   ├── EmployeeForm.jsx          # Add / edit employee modal form
│       │   │   │   └── EmployeeList.jsx          # Searchable employee table
│       │   │   └── leave/
│       │   │       ├── LeaveForm.jsx             # Submit leave request modal form
│       │   │       ├── LeaveList.jsx             # Leave requests data table
│       │   │       └── LeaveActions.jsx          # Approve / reject / cancel buttons
│       │   │
│       │   ├── context/
│       │   │   └── AuthContext.jsx              # Global auth state (user, token, roles)
│       │   │
│       │   ├── hooks/
│       │   │   ├── useAuth.js                   # Hook to consume AuthContext
│       │   │   └── usePagination.js             # Hook for page state management
│       │   │
│       │   ├── pages/
│       │   │   ├── LoginPage.jsx                # Login form page
│       │   │   ├── DashboardPage.jsx            # Stats overview (role-specific)
│       │   │   ├── EmployeesPage.jsx            # Admin employee management page
│       │   │   └── LeaveRequestsPage.jsx        # Leave management page
│       │   │
│       │   ├── routes/
│       │   │   ├── AppRoutes.jsx                # Route definitions
│       │   │   └── ProtectedRoute.jsx           # Auth + role guard component
│       │   │
│       │   ├── utils/
│       │   │   ├── dateUtils.js                 # Date formatting helpers
│       │   │   └── constants.js                 # ROLES, LEAVE_STATUS, STATUS_COLORS enums
│       │   │
│       │   ├── App.jsx
│       │   ├── main.jsx
│       │   └── index.css                        # Tailwind CSS v4 base import
│       │
│       ├── public/
│       ├── vite.config.js                       # Vite + /api proxy to backend
│       ├── tailwind.config.js
│       ├── package.json
│       ├── .env                                 # VITE_API_BASE_URL=/api
│       └── index.html
│
├── LeaveManagement.sln                          # Visual Studio solution file
└── README.md
```

---

## Database Schema

The database is PostgreSQL. EF Core Fluent API maps entities to snake_case column names.

### Table: `roles`

| Column | Type | Constraints |
|---|---|---|
| `id` | `integer` | PK, auto-increment |
| `role_name` | `varchar` | NOT NULL, UNIQUE (`Admin`, `Employee`) |
| `created_at` | `timestamp` | NOT NULL, default now |

> Seeded on first migration: `Admin` (id=1), `Employee` (id=2)

---

### Table: `users`

| Column | Type | Constraints |
|---|---|---|
| `id` | `integer` | PK, auto-increment |
| `first_name` | `varchar` | NOT NULL |
| `last_name` | `varchar` | NOT NULL |
| `email` | `varchar` | NOT NULL, UNIQUE |
| `password_hash` | `varchar` | NOT NULL (BCrypt) |
| `role_id` | `integer` | FK → `roles.id` (RESTRICT delete) |
| `department` | `varchar` | nullable |
| `designation` | `varchar` | nullable |
| `date_of_joining` | `date` | nullable |
| `is_active` | `boolean` | NOT NULL, default `true` |
| `created_at` | `timestamp` | NOT NULL |
| `updated_at` | `timestamp` | NOT NULL |

---

### Table: `leave_requests`

| Column | Type | Constraints |
|---|---|---|
| `id` | `integer` | PK, auto-increment |
| `employee_id` | `integer` | FK → `users.id` (CASCADE delete) |
| `from_date` | `date` | NOT NULL |
| `to_date` | `date` | NOT NULL |
| `reason` | `varchar` | NOT NULL (5–1000 chars) |
| `status` | `leavestatus` | PostgreSQL ENUM (`Pending`, `Approved`, `Rejected`) |
| `reviewed_by` | `integer` | FK → `users.id` (SET NULL on delete), nullable |
| `reviewed_at` | `timestamp` | nullable |
| `remarks` | `varchar` | nullable (max 500 chars) |
| `created_at` | `timestamp` | NOT NULL |
| `updated_at` | `timestamp` | NOT NULL |

**Entity Relationships:**
```
roles ──< users >──< leave_requests
                        ↑ reviewed_by (nullable FK → users)
```

---

## Backend Architecture

The backend follows a clean **Controller → Service → Repository (EF Core)** layered architecture.

### Request Pipeline

```
HTTP Request
    ↓
ExceptionMiddleware          (global error handling)
    ↓
JWT Authentication           (validates Bearer token)
    ↓
Authorization                (role-based [Authorize] attributes)
    ↓
Controller                   (validates input DTOs, calls service)
    ↓
Service                      (business logic, EF Core queries)
    ↓
AppDbContext (EF Core)       (maps to PostgreSQL)
    ↓
HTTP Response (ApiResponse<T>)
```

### Services Overview

#### `AuthService`
- **Login**: Looks up user by email → verifies BCrypt hash → generates JWT via `JwtService`
- **Register**: Validates role exists, checks email uniqueness → creates user with BCrypt-hashed password

#### `EmployeeService`
- **GetAll**: Returns paginated list of active employees
- **GetById**: Returns single employee (used for admin lookup and self-profile)
- **Create**: Creates new user with role `Employee` automatically assigned
- **Update**: Partial update — only non-null fields are applied
- **Delete**: Soft-delete — sets `IsActive = false` (data is preserved)

#### `LeaveRequestService`
- **GetAll** (admin): Returns all leave requests, optional `status` filter, paginated
- **GetMyLeaves** (employee): Returns only the requesting user's leaves
- **GetById**: Returns single leave request; employees can only retrieve their own
- **Create**: Validates employee is active, start date is not in the past, end ≥ start, no overlapping Pending/Approved leave exists for the same date range
- **Approve / Reject**: Admin-only; records reviewer id, timestamp, and optional remarks
- **Cancel**: Employee can cancel their own leave only while status is still `Pending`

#### `JwtService`
Generates a signed HS256 JWT containing claims: `userId`, `email`, `role`, `fullName`, `jti`. Token validity is configured in `appsettings.json` (default 480 minutes).

### Generic API Response Wrapper

All endpoints return:
```json
{
  "success": true,
  "message": "string",
  "data": { },
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalCount": 42,
    "totalPages": 5
  }
}
```

---

## API Endpoints Reference

### Auth — `/api/auth`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/login` | Public | Login; returns JWT + user info |
| `POST` | `/api/auth/register` | Admin | Register a new user |

**Login request:**
```json
{
  "email": "admin@company.com",
  "password": "Admin@123"
}
```

**Login response:**
```json
{
  "token": "<jwt>",
  "role": "Admin",
  "userId": 1,
  "fullName": "System Admin",
  "email": "admin@company.com",
  "expiresAt": "2025-01-01T16:00:00Z"
}
```

---

### Employees — `/api/employees`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/employees` | Admin | List all employees (paginated) |
| `GET` | `/api/employees/{id}` | Admin or Self | Get employee by ID |
| `POST` | `/api/employees` | Admin | Create new employee |
| `PUT` | `/api/employees/{id}` | Admin | Update employee fields |
| `DELETE` | `/api/employees/{id}` | Admin | Soft-delete (deactivate) employee |

---

### Leave Requests — `/api/leaverequests`

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `GET` | `/api/leaverequests` | Admin | All leave requests; filter by `?status=Pending` |
| `GET` | `/api/leaverequests/my` | Employee | Current user's own leaves |
| `GET` | `/api/leaverequests/{id}` | Admin or Owner | Get single leave request |
| `POST` | `/api/leaverequests` | Employee | Submit new leave request |
| `PUT` | `/api/leaverequests/{id}/approve` | Admin | Approve a pending leave |
| `PUT` | `/api/leaverequests/{id}/reject` | Admin | Reject a pending leave |
| `DELETE` | `/api/leaverequests/{id}` | Employee (Owner) | Cancel own pending leave |

**Create leave request:**
```json
{
  "fromDate": "2025-02-10",
  "toDate": "2025-02-12",
  "reason": "Family vacation"
}
```

**Approve/Reject payload:**
```json
{
  "remarks": "Approved as per policy"
}
```

---

## Frontend Architecture

The frontend is a React SPA built with Vite. All API calls go through an Axios instance that automatically attaches the JWT Authorization header from `localStorage`.

### Routing

```
/                 → redirects to /dashboard (if authenticated) or /login
/login            → LoginPage (public)
/dashboard        → DashboardPage (protected, all roles)
/employees        → EmployeesPage (protected, Admin only)
/leave-requests   → LeaveRequestsPage (protected, all roles)
```

**`ProtectedRoute`** checks: is user logged in? Does user have the required role? Redirects to `/login` if not authenticated, or shows a 403 message if wrong role.

### State Management

Global auth state is managed via React Context (`AuthContext`). It holds:
- `user` object (id, fullName, email, role)
- `token` (JWT string)
- `login(token, user)` — stores to `localStorage` under keys `lms_token` / `lms_user`
- `logout()` — clears localStorage and redirects to `/login`
- `isAdmin()` / `isEmployee()` — convenience role checks

### Axios Interceptor

`axiosInstance.js` attaches the JWT token to every outbound request:
```js
request.headers.Authorization = `Bearer ${token}`
```
On a `401 Unauthorized` response, the interceptor clears auth state and redirects to `/login` automatically.

### Page-Level Components

#### LoginPage
- Email + password form
- On success: stores token + user → redirects to `/dashboard`
- Displays default credentials for demo purposes

#### DashboardPage
- **Admin view**: Total employees count, total leave requests, leaves by status (Pending / Approved / Rejected)
- **Employee view**: Own leave counts by status, personal leave history summary

#### EmployeesPage *(Admin only)*
- Searchable, paginated table of all employees
- **Add Employee**: Modal form with fields — First Name, Last Name, Email, Password, Department, Designation, Date of Joining
- **Edit Employee**: Same modal pre-filled; password field hidden on edit
- **Deactivate**: Confirmation dialog before soft-delete

#### LeaveRequestsPage
- **Admin view**: All employees' leave requests; filter by status tab (All / Pending / Approved / Rejected); Approve or Reject with remarks
- **Employee view**: Own leave history; Submit New Leave Request button; Cancel pending leaves

### Form Validation (Yup schemas)

| Form | Validations |
|---|---|
| Login | Email format, password required |
| Register/Add Employee | Strong password (8+ chars, uppercase, lowercase, digit, special char), email format |
| Leave Request | `fromDate` ≥ today, `toDate` ≥ `fromDate`, reason 5–1000 characters |
| Approve/Reject | Remarks optional, max 500 characters |

---

## Role-Based Features

### Admin

| Feature | Description |
|---|---|
| Register employees | Create accounts for new employees via form |
| View all employees | Paginated table with search by name/email |
| Edit employee profile | Update department, designation, joining date, active status |
| Deactivate employee | Soft-delete — employee can no longer log in |
| View all leave requests | Filterable by status |
| Approve leave | Marks leave `Approved`; records reviewer + timestamp |
| Reject leave | Marks leave `Rejected` with optional remarks |
| Dashboard statistics | Total employees, leave counts by status |

### Employee

| Feature | Description |
|---|---|
| Login | Access own portal |
| Submit leave request | Date range + reason; business rules enforced |
| View own leaves | Full history with status |
| Cancel pending leave | Only while status is `Pending` |
| Dashboard | Personal leave statistics |

---

## Authentication Flow

```
1. User submits email + password on /login
         ↓
2. POST /api/auth/login
         ↓
3. AuthService: find user by email → BCrypt.Verify(password, hash)
         ↓
4. JwtService: generate signed HS256 token with claims
   { userId, email, role, fullName, jti, exp }
         ↓
5. Token returned to frontend
         ↓
6. Frontend stores token in localStorage (lms_token, lms_user)
         ↓
7. AuthContext updates global state
         ↓
8. React Router redirects to /dashboard
         ↓
9. All subsequent API requests include:
   Authorization: Bearer <token>
         ↓
10. On 401 response → auto-logout + redirect to /login
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/download/) running on port `5432`

---

### 1. Database Setup

Create a PostgreSQL database:
```sql
CREATE DATABASE "LeaveManagementDB";
```

---

### 2. Backend Setup

```bash
cd Backend/LeaveManagement/LeaveManagement.API
```

Update the connection string in `appsettings.json` to match your PostgreSQL credentials:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=LeaveManagementDB;Username=postgres;Password=YOUR_PASSWORD"
}
```

Run EF Core migrations to create tables and seed roles:
```bash
dotnet ef database update
```

Start the API:
```bash
dotnet run
```

API will be available at `http://localhost:5000`  
Swagger UI: `http://localhost:5000/swagger`

---

### 3. Frontend Setup

```bash
cd Frontend/leave-management-ui
npm install
npm run dev
```

The app will run at `http://localhost:5173`. All `/api` requests are proxied to the backend at `http://localhost:5000`.

---

## Environment Configuration

### Backend — `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LeaveManagementDB;Username=postgres;Password=YOUR_PASSWORD"
  },
  "JwtSettings": {
    "SecretKey": "LeaveManagement$SuperSecret#Key2025!MustBe32Chars",
    "Issuer": "LeaveManagementAPI",
    "Audience": "LeaveManagementClient",
    "ExpiryInMinutes": "480"
  }
}
```

### Frontend — `.env`

```
VITE_API_BASE_URL=/api
```

> The `/api` prefix is proxied to `http://localhost:5000` in `vite.config.js` during development.

---

## Running Tests

The test project uses xUnit with an in-memory EF Core database and Moq for mocking.

```bash
cd Backend/LeaveManagement
dotnet test
```

**Test coverage includes:**

| Test File | What it tests |
|---|---|
| `AuthServiceTests.cs` | Login success/failure, registration, duplicate email, invalid role |
| `EmployeeServiceTests.cs` | CRUD operations, pagination, soft-delete, access control |
| `LeaveRequestServiceTests.cs` | Leave creation validation (overlaps, past dates, inactive employees), approve/reject/cancel flows |

Test helpers:
- `DbContextFactory` — creates isolated in-memory `AppDbContext` per test
- `JwtConfigFactory` — builds test-safe JWT configuration
- `SeedData` — shared entity seed data for consistent test setup

---

## Default Credentials

> These credentials are seeded via the first admin registration. Use them to log in and explore the system.

| Role | Email | Password |
|---|---|---|
| Admin | `admin@company.com` | `Admin@123` |

Once logged in as Admin, you can register Employee accounts via the Employees page.

---

## Key Business Rules

1. **Leave date validation**: Start date must not be in the past; end date must be ≥ start date.
2. **Overlap prevention**: Employees cannot submit a leave that overlaps with any existing `Pending` or `Approved` leave.
3. **Cancel restriction**: Employees can only cancel leaves that are still in `Pending` status.
4. **Soft delete**: Deleting an employee sets `IsActive = false`. Their data and leave history are preserved. They can no longer log in.
5. **Access control**: Employees can only view, submit, or cancel their own leave requests. They cannot access other employees' data.
6. **Strong passwords**: Registration requires a minimum 8-character password containing at least one uppercase letter, one lowercase letter, one digit, and one special character.
