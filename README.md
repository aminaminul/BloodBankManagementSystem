# 🩸 Blood Bank & Emergency Healthcare Management System (BBDMS)

A modern, full-featured web-based platform built on **ASP.NET Core MVC (.NET 10)** and **Entity Framework Core**. It integrates voluntary blood donation, patient emergency blood requests, real-time hospital ICU and emergency bed tracking, 24/7 ambulance dispatch, medical oxygen gas supplies, and an administrative control panel.

---

## 🌟 Key Features

### 🚑 Emergency Healthcare Hub
- **Hospital & ICU Live Bed Tracker**: Check real-time emergency bed and ICU bed availability across regional medical centers.
- **Blood Bank Directory**: Explore accredited blood banks with real-time blood group stock indicators.
- **24/7 Ambulance Fleets**: Browse Advanced Life Support (ALS/ICU), Basic Life Support (BLS), and Neonatal transport units with instant dispatch request forms.
- **Medical Oxygen Services**: Directory of medical gas cylinder suppliers and home delivery refills.
- **Emergency Blood Request Board**: Public emergency bulletin allowing patients to submit urgent blood requests with automated urgency categorization (Critical, Urgent, Standard).

### 👥 Donor Portal
- **Donor Registration & Authentication**: Secure registration with BCrypt password hashing and duplicate email prevention.
- **Medical Eligibility Tracker**: Automated tracking adhering to the 90-day minimum waiting period between blood donations with real-time countdown.
- **Donation Availability Toggle**: Donors can update their availability (`Available` vs `Unavailable`) anytime.
- **Personalized Request Feed**: Donors receive direct blood requests as well as community requests matching their blood group.

### 🛡️ Administrative Control Center
- **Protected by `[AdminAuthorize]` filter** and session security hardening.
- **Full CRUD Management**: Hospitals & ICU beds, Blood Banks, Ambulance fleet records & dispatch bookings, Oxygen suppliers, CMS pages, and Contact inquiries.
- **Anti-CSRF Protection**: All destructive actions (delete, status updates) are strictly POST requests with AntiForgery tokens.
- **Server-Side Pagination & Real-Time Filtering**: High-performance paging and instant client-side table search.

---

## 🛠️ Technology Stack

- **Framework**: .NET 10.0 (ASP.NET Core MVC)
- **ORM**: Entity Framework Core 10.0
- **Database**: Microsoft SQL Server / LocalDB (`(localdb)\MSSQLLocalDB`)
- **Security**: `BCrypt.Net-Next` (adaptive password hashing)
- **Architecture**: 4-Layer Clean Architecture (`Model` → `Repository` → `Service` → `Web`)
- **Frontend**: Bootstrap 4, FontAwesome 5, jQuery

---

## 🚀 Getting Started

### Prerequisites
1. [.NET 10 SDK](https://dotnet.microsoft.com/download)
2. Microsoft SQL Server LocalDB or full SQL Server instance

### Setup & Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/BloodBankManagementSystem.git
   cd BloodBankManagementSystem
   ```

2. **Verify Database Connection**:
   In `BBDMS.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BloodBankMSDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. **Apply Database Migrations**:
   ```bash
   dotnet ef database update --project BBDMS.Repository --startup-project BBDMS.Web
   ```

4. **Build & Run**:
   ```bash
   dotnet build BBDMS.sln
   dotnet run --project BBDMS.Web
   ```

5. **Access Application**:
   - Web application runs at: `http://localhost:5148`

---

## 🔑 Default Credentials (Seeded Automatically)

| Role | Username / Email | Password |
|---|---|---|
| **Administrator** | `admin` | `admin` |
| **Sample Donor 1** | `tanvir@example.com` | `donor` |
| **Sample Donor 2** | `nusrat@example.com` | `donor` |

*(Note: Passwords are automatically hashed via BCrypt on initial startup.)*

---

## 🔒 Security Highlights

- **BCrypt Password Hashing**: Passwords are protected using BCrypt with adaptive cost factor. Legacy plain-text passwords auto-upgrade on first login.
- **Strict Anti-CSRF**: All delete and modification operations require valid HTTP POST Anti-Forgery tokens.
- **Hardened Cookies**: Session and Anti-Forgery cookies configured with `HttpOnly = true`, `SameSite = Strict`, and 30-minute idle expiration.
- **SQL-Level Aggregation**: In-memory N+1 bottlenecks eliminated in favor of database-level `COUNT(*)`, `SUM()`, and server-side pagination.