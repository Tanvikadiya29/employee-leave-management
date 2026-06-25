# User Interface Guide — Employee Leave Management System

This guide explains every screen, page, and interaction available in the Employee Leave Management System. It is written for end users — both **Admins** and **Employees** — and covers how to navigate the application, what each page does, and the step-by-step flows for every action.

---

## Table of Contents

1. [Accessing the Application](#accessing-the-application)
2. [User Roles at a Glance](#user-roles-at-a-glance)
3. [Login Page](#login-page)
4. [Application Layout](#application-layout)
   - [Navbar](#navbar)
   - [Sidebar](#sidebar)
5. [Dashboard Page](#dashboard-page)
   - [Admin Dashboard](#admin-dashboard)
   - [Employee Dashboard](#employee-dashboard)
6. [Employees Page (Admin Only)](#employees-page-admin-only)
   - [Viewing Employees](#viewing-employees)
   - [Adding a New Employee](#adding-a-new-employee)
   - [Editing an Employee](#editing-an-employee)
   - [Deactivating an Employee](#deactivating-an-employee)
7. [Leave Requests Page](#leave-requests-page)
   - [Admin — Viewing All Leave Requests](#admin--viewing-all-leave-requests)
   - [Admin — Approving a Leave Request](#admin--approving-a-leave-request)
   - [Admin — Rejecting a Leave Request](#admin--rejecting-a-leave-request)
   - [Employee — Viewing Own Leave Requests](#employee--viewing-own-leave-requests)
   - [Employee — Submitting a New Leave Request](#employee--submitting-a-new-leave-request)
   - [Employee — Cancelling a Leave Request](#employee--cancelling-a-leave-request)
8. [Status Badges](#status-badges)
9. [Notifications and Feedback](#notifications-and-feedback)
10. [Pagination](#pagination)
11. [Logging Out](#logging-out)
12. [Business Rules and Validations](#business-rules-and-validations)
13. [Quick Reference — What Each Role Can Do](#quick-reference--what-each-role-can-do)
14. [Troubleshooting Common Issues](#troubleshooting-common-issues)

---

## Accessing the Application

Open your browser and navigate to:

```
http://localhost:5173
```

> If deployed to a production server, your administrator will provide you with the hosted URL.

If you are not logged in, the application will automatically redirect you to the **Login Page**.

---

## User Roles at a Glance

The system has two roles. Every user belongs to exactly one.

| Role | Who it is | What they can do |
|---|---|---|
| **Admin** | HR manager, team lead, or system administrator | Manage all employees, view and act on all leave requests, see full statistics |
| **Employee** | Any staff member registered by an Admin | Submit own leave requests, view own leave history, cancel pending requests |

> Your role is assigned at registration and is displayed in the top navigation bar. You cannot change your own role.

---

## Login Page

**URL:** `/login`  
**Access:** Public — no login required

This is the first screen you see when opening the application or after your session expires.

### What you see

- **Email** input field
- **Password** input field
- **Sign In** button
- Default credential hint displayed below the form (for demo environments)

### How to log in

1. Enter your registered **email address** in the Email field.
2. Enter your **password** in the Password field.
3. Click **Sign In**.

On success, you are redirected to the **Dashboard**.  
On failure, an error message appears below the form describing the issue (e.g., invalid credentials, account inactive).

### Default demo credentials

| Role | Email | Password |
|---|---|---|
| Admin | `admin@company.com` | `Admin@123` |

> Employee accounts are created by the Admin. Ask your Admin for your login credentials.

### Session duration

Your session lasts **8 hours** from the time of login. After expiry, you are automatically redirected to the login page and must sign in again.

---

## Application Layout

After logging in, every page shares a consistent layout with two persistent navigation elements.

```
┌──────────────────────────────────────────────────────┐
│                      NAVBAR                          │
├──────────┬───────────────────────────────────────────┤
│          │                                           │
│ SIDEBAR  │           PAGE CONTENT                    │
│          │                                           │
│          │                                           │
│          │                                           │
└──────────┴───────────────────────────────────────────┘
```

---

### Navbar

The **Navbar** runs across the top of every page.

| Element | Description |
|---|---|
| Application name / logo | Displayed on the left |
| Logged-in user's full name | Displayed on the right |
| User's role badge | Shown next to the name (`Admin` or `Employee`) |
| **Logout** button | Top-right corner; ends your session immediately |

---

### Sidebar

The **Sidebar** runs down the left side of every page. It contains navigation links.

**Admin sidebar links:**

| Link | Page it opens |
|---|---|
| Dashboard | Overview statistics |
| Employees | Employee management |
| Leave Requests | All leave requests |

**Employee sidebar links:**

| Link | Page it opens |
|---|---|
| Dashboard | Personal statistics |
| Leave Requests | Own leave history + submission |

> The **Employees** link is hidden from Employee accounts. Attempting to access `/employees` directly as an Employee will display an access-denied message.

The currently active page is highlighted in the sidebar.

---

## Dashboard Page

**URL:** `/dashboard`  
**Access:** All logged-in users (Admin and Employee)

The Dashboard is the first page you land on after logging in. It provides a quick statistical overview of the system, tailored to your role.

---

### Admin Dashboard

Admins see a company-wide summary:

| Statistic | Description |
|---|---|
| Total Employees | Count of all active registered employees |
| Total Leave Requests | Combined count of all leave requests across all statuses |
| Pending Requests | Leaves awaiting admin review |
| Approved Requests | Leaves that have been approved |
| Rejected Requests | Leaves that have been rejected |

Use the dashboard to get an at-a-glance sense of current workload — how many requests are waiting for your review — before navigating to the Leave Requests page to act on them.

---

### Employee Dashboard

Employees see a personal summary:

| Statistic | Description |
|---|---|
| My Pending Requests | Own leaves awaiting admin decision |
| My Approved Requests | Own leaves that have been approved |
| My Rejected Requests | Own leaves that have been rejected |
| Total Requests Submitted | Lifetime count of own submitted requests |

Use the dashboard to check the status of your recent submissions at a glance before navigating to Leave Requests for the full history.

---

## Employees Page (Admin Only)

**URL:** `/employees`  
**Access:** Admin only

This page is the central hub for managing all employee accounts. Employees cannot access this page.

---

### Viewing Employees

When you open the Employees page, you see a **table** listing all active employees with the following columns:

| Column | Description |
|---|---|
| Name | Employee's full name |
| Email | Registered email address |
| Department | Department they belong to (if set) |
| Designation | Job title (if set) |
| Date of Joining | When they joined (if set) |
| Status | Active / Inactive badge |
| Actions | Edit and Deactivate buttons |

**Search:** A search bar above the table lets you filter employees by name or email in real time. Type any part of the name or email and the table updates instantly.

**Pagination:** If there are more employees than fit on one page, pagination controls appear at the bottom of the table. See the [Pagination](#pagination) section for details.

---

### Adding a New Employee

To register a new employee in the system:

1. Click the **Add Employee** button (top-right of the Employees page).
2. A modal form slides open with the following fields:

| Field | Required | Notes |
|---|---|---|
| First Name | Yes | Letters only |
| Last Name | Yes | Letters only |
| Email | Yes | Must be a valid, unique email address |
| Password | Yes | Minimum 8 characters; must include uppercase, lowercase, digit, and special character |
| Department | No | Free-text field (e.g., Engineering, HR) |
| Designation | No | Free-text field (e.g., Senior Developer) |
| Date of Joining | No | Date picker |

3. Fill in the required fields.
4. Click **Save** (or **Create Employee**).

**On success:** The modal closes, a success toast notification appears, and the new employee appears in the table.  
**On failure:** Validation errors are shown inline beneath the relevant field (e.g., "Email already in use", "Password too weak").

> The new employee can now log in immediately using the email and password you set.

---

### Editing an Employee

To update an employee's information:

1. Find the employee in the table (use search if needed).
2. Click the **Edit** button (pencil icon) in their row.
3. The same modal form opens, pre-filled with their current data.

> The password field is hidden when editing — passwords cannot be changed from this form.

4. Modify any of the following fields:
   - First Name
   - Last Name
   - Department
   - Designation
   - Date of Joining
   - Active status (toggle to reactivate a deactivated employee)

5. Click **Save**.

**On success:** The modal closes and the table row updates with the new data.

---

### Deactivating an Employee

Deactivating an employee disables their account. They can no longer log in, but all their data and leave history remain intact in the system.

1. Find the employee in the table.
2. Click the **Deactivate** button (trash / deactivate icon) in their row.
3. A **confirmation dialog** appears:
   > "Are you sure you want to deactivate this employee? They will no longer be able to log in."
4. Click **Confirm** to proceed, or **Cancel** to abort.

**On confirm:** The employee's row is removed from the active list and a success notification appears.

> To reactivate a deactivated employee, use the **Edit** form and toggle their status back to Active.

---

## Leave Requests Page

**URL:** `/leave-requests`  
**Access:** All logged-in users (Admin and Employee — different views)

This page behaves differently depending on your role.

---

### Admin — Viewing All Leave Requests

Admins see every leave request submitted by all employees.

The table displays:

| Column | Description |
|---|---|
| Employee Name | Who submitted the request |
| From Date | Leave start date |
| To Date | Leave end date |
| Total Days | Calculated number of calendar days |
| Reason | The employee's stated reason |
| Status | Current status badge (Pending / Approved / Rejected) |
| Submitted On | Date the request was created |
| Reviewed By | Admin who acted on it (if applicable) |
| Remarks | Admin's comments on approval/rejection (if provided) |
| Actions | Approve / Reject buttons (visible on Pending requests) |

**Filtering by status:**  
Tab buttons at the top of the table allow you to filter:
- **All** — shows every request regardless of status
- **Pending** — shows only requests awaiting review
- **Approved** — shows only approved requests
- **Rejected** — shows only rejected requests

Click any tab to switch the view instantly.

---

### Admin — Approving a Leave Request

1. Navigate to the **Leave Requests** page.
2. Optionally click the **Pending** tab to filter for requests needing attention.
3. Find the leave request you want to approve.
4. Click the **Approve** button (green checkmark) in the Actions column.
5. A modal appears prompting for optional **Remarks**:
   > "Enter any comments for the employee (optional)"
6. Type remarks if desired, or leave the field blank.
7. Click **Confirm Approve**.

**On success:** The leave status changes to **Approved** (green badge). The Approve/Reject buttons disappear from that row. The reviewer's name and timestamp are recorded.

---

### Admin — Rejecting a Leave Request

1. Navigate to the **Leave Requests** page.
2. Find the pending leave request.
3. Click the **Reject** button (red X) in the Actions column.
4. A modal appears prompting for optional **Remarks**:
   > "Please provide a reason for rejection (optional)"
5. Type remarks if desired.
6. Click **Confirm Reject**.

**On success:** The leave status changes to **Rejected** (red badge). The Approve/Reject buttons disappear. Remarks are saved and visible to the employee.

> Only **Pending** leaves can be approved or rejected. Once a leave is Approved or Rejected, it cannot be changed.

---

### Employee — Viewing Own Leave Requests

Employees see only their own leave history. The table displays:

| Column | Description |
|---|---|
| From Date | Leave start date |
| To Date | Leave end date |
| Total Days | Number of calendar days |
| Reason | The reason you submitted |
| Status | Current status badge |
| Submitted On | When you submitted it |
| Remarks | Admin's comments (if provided) |
| Actions | Cancel button (visible only on Pending requests) |

---

### Employee — Submitting a New Leave Request

1. Navigate to the **Leave Requests** page.
2. Click the **New Leave Request** button (top-right of the page).
3. A modal form opens with the following fields:

| Field | Required | Notes |
|---|---|---|
| From Date | Yes | Cannot be in the past; must be today or a future date |
| To Date | Yes | Must be equal to or after the From Date |
| Reason | Yes | Between 5 and 1000 characters |

4. Select your leave dates using the date picker controls.
5. Type your reason in the Reason field.
6. Click **Submit Request**.

**On success:** The modal closes, a success toast appears, and your new request appears in the table with status **Pending**.

**Validation errors you may see:**

| Error | Meaning |
|---|---|
| "Start date cannot be in the past" | You selected a date before today |
| "End date must be after or equal to start date" | To Date is earlier than From Date |
| "Reason must be at least 5 characters" | Reason field too short |
| "You already have an overlapping leave request" | A Pending or Approved leave already exists for the selected date range |

> After submission, your request is visible to the Admin on their Leave Requests page. You will need to check back or contact your Admin to know when it has been reviewed.

---

### Employee — Cancelling a Leave Request

You can only cancel a leave request while it is still in **Pending** status. Once approved or rejected, it cannot be cancelled.

1. Find the pending leave request in your table.
2. Click the **Cancel** button in the Actions column.
3. A **confirmation dialog** appears:
   > "Are you sure you want to cancel this leave request? This action cannot be undone."
4. Click **Confirm** to cancel, or **Cancel** to go back.

**On success:** The leave request is removed from your list and a success notification appears.

---

## Status Badges

Every leave request displays a color-coded status badge that indicates its current state.

| Badge | Color | Meaning |
|---|---|---|
| **Pending** | Yellow | The request has been submitted and is waiting for admin review |
| **Approved** | Green | The admin has approved the leave |
| **Rejected** | Red | The admin has rejected the leave |

---

## Notifications and Feedback

The application shows **toast notifications** (small pop-up messages) in the top-right corner of the screen to confirm actions or report errors.

| Notification type | Color | When it appears |
|---|---|---|
| Success | Green | Action completed successfully (e.g., leave submitted, employee saved) |
| Error | Red | Something went wrong (e.g., validation failure, server error) |
| Warning | Yellow | Action requires attention (e.g., trying to cancel a non-pending leave) |

Notifications disappear automatically after a few seconds or can be dismissed by clicking the X on them.

**Inline form errors** appear directly beneath the relevant input field in red text when you submit a form with invalid data. Fix the highlighted fields and resubmit.

---

## Pagination

Tables with many records are split across pages. Pagination controls appear at the bottom of the table.

```
  ← Previous    Page 2 of 7    Next →
  Showing 11–20 of 68 records
```

| Control | Action |
|---|---|
| **Previous** button | Go to the previous page |
| **Next** button | Go to the next page |
| Page indicator | Shows current page and total pages |
| Record count | Shows which records are visible out of the total |

Each page shows a maximum of **10 records** by default.

---

## Logging Out

To end your session:

1. Click the **Logout** button in the top-right corner of the Navbar.
2. You are immediately signed out and redirected to the **Login Page**.

Your session data is cleared from the browser. The next person who opens the application on that browser will be shown the login screen.

> If your session expires naturally (after 8 hours of inactivity), the application will redirect you to the Login Page automatically the next time you try to perform an action.

---

## Business Rules and Validations

The system enforces the following rules automatically. Attempting to violate them results in a clear error message.

### Leave Request Rules

| Rule | Details |
|---|---|
| No past-dated leaves | The start date of a leave must be today or in the future |
| End ≥ Start | The end date must be the same as or later than the start date |
| No overlapping leaves | You cannot submit a leave that overlaps (even partially) with an existing Pending or Approved leave |
| Reason required | Every leave request must include a reason (5 to 1000 characters) |
| Cancel only Pending | A leave can only be cancelled while its status is Pending |
| Approve/Reject only Pending | Admins can only approve or reject Pending requests |

### Employee Account Rules

| Rule | Details |
|---|---|
| Unique email | No two users can share the same email address |
| Strong password | Minimum 8 characters; must contain at least one uppercase letter, one lowercase letter, one number, and one special character |
| Inactive accounts cannot log in | Deactivated employees are blocked at login |
| Inactive employees cannot submit leaves | Even if an account is somehow accessed, the backend rejects leave submissions from inactive users |

---

## Quick Reference — What Each Role Can Do

### Admin

| Action | Where |
|---|---|
| View company-wide leave statistics | Dashboard |
| View all employees | Employees page |
| Add a new employee | Employees page → Add Employee button |
| Edit an employee's profile | Employees page → Edit button |
| Deactivate an employee | Employees page → Deactivate button |
| View all leave requests | Leave Requests page |
| Filter leave requests by status | Leave Requests page → status tabs |
| Approve a pending leave | Leave Requests page → Approve button |
| Reject a pending leave | Leave Requests page → Reject button |
| Logout | Navbar → Logout button |

### Employee

| Action | Where |
|---|---|
| View personal leave statistics | Dashboard |
| View own leave request history | Leave Requests page |
| Submit a new leave request | Leave Requests page → New Leave Request button |
| Cancel a pending leave request | Leave Requests page → Cancel button |
| Logout | Navbar → Logout button |

---

## Troubleshooting Common Issues

**I cannot log in.**
- Double-check your email address for typos.
- Ensure Caps Lock is not on when entering your password.
- If you have forgotten your password, contact your Admin — they will need to create a new account for you.
- If you see "Account is inactive", your account has been deactivated. Contact your Admin.

**I submitted a leave but it's not showing up.**
- Refresh the page. The table may not have updated automatically.
- Check that the submission toast notification appeared green (success). If it was red, the request was not saved — read the error message and retry.

**The Approve/Reject buttons are missing on a leave request.**
- These buttons only appear on **Pending** requests. If the leave is already Approved or Rejected, the buttons are hidden by design.

**I cannot cancel my leave.**
- The Cancel button only appears on your own **Pending** leaves. If your leave has already been Approved or Rejected, it can no longer be cancelled. Contact your Admin if you need to make a change.

**I am getting "overlapping leave" error when submitting.**
- You already have a Pending or Approved leave that covers some or all of the dates you selected. Go to your leave history table to check which dates are already taken, then choose a non-overlapping date range.

**The Employees page is not visible in my sidebar.**
- The Employees page is only accessible to **Admin** accounts. If you are an Employee, this page is intentionally hidden. Contact your Admin if you believe this is an error.

**My session expired unexpectedly.**
- Sessions last 8 hours. If you were inactive for an extended period, you will need to log in again. This is a security feature.

**The page shows a loading spinner but never loads.**
- Check that the backend API is running at `http://localhost:5000`. If you are in a production environment, contact your system administrator.
