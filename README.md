# 🩸 Blood Bank & Emergency Healthcare Management System (BBDMS)

A full-featured, enterprise-grade web platform engineered with **ASP.NET Core MVC (.NET 10)**, **Entity Framework Core 10**, and **SignalR**. BBDMS unifies voluntary blood donation, patient emergency blood requests, real-time hospital ICU and emergency bed tracking, 24/7 ambulance dispatch, medical oxygen supplies, and a comprehensive administrative control center into a single ecosystem.

---

## 🌟 Key Features

### 🚑 Emergency Healthcare Hub
- **Interactive Leaflet / OpenStreetMap Healthcare Map**: Live geolocation map displaying regional hospitals, accredited blood banks, and ambulance fleets with category filters and interactive status popups.
- **Hospital & ICU Live Bed Tracker**: Real-time tracking of general emergency beds, ICU beds, trauma units, and specialized facilities across medical centers.
- **24/7 Ambulance Fleet & Dispatch**: Advanced Life Support (ALS/ICU), Basic Life Support (BLS), and Neonatal ambulance units with instant online booking.
- **Token-Based Live Ambulance Tracking**: 128-bit unguessable tracking tokens with animated progress timelines and live status updates.
- **Medical Oxygen Supply Directory**: Verified medical gas suppliers, cylinder refill centers, and home delivery availability indicators.
- **Emergency Blood Request Bulletin**: Urgent blood requests board with automated triage urgency categorization (Critical, Urgent, Standard).
- **Instant Social Emergency Broadcast**: One-click emergency broadcast to WhatsApp, Telegram, and mobile Web Share API for rapid community mobilization.
- **Critical Reserve Low-Stock Alert**: Prominent alert banners that highlight critically depleted blood groups across regional blood banks.

### 👥 Donor Portal & Digital Health Ecosystem
- **Digital Donor Card**: Personal digital donor identity card featuring blood group badges, donation milestones, and an automated QR code.
- **Public QR Verification Checkpoint**: Publicly accessible verification endpoint for hospitals and clinics to scan and verify donor identity and donation records.
- **Smart "Next Eligible Date" Calendar Reminder (`.ics`)**: Automated 90-day whole-blood interval calculator that generates downloadable `.ics` calendar events compatible with Google Calendar, Apple Calendar, and Microsoft Outlook.
- **Donor Milestones & Printable Certificate of Appreciation**: Dynamic milestone tiers (Registered Life Saver, Bronze Life Saver, Silver Guardian, Gold Champion, Platinum Legend) with print-ready, gold-accented Certificates of Appreciation.
- **Self-Service Password Reset Workflow**: Secure, tokenized password reset system with expiring cryptographic tokens and enumeration-resistant messaging.
- **Blood Compatibility Matrix & 60-Second Eligibility Quiz**: Visual red blood cell and plasma compatibility explorer paired with an interactive 6-question donor pre-screening questionnaire.
- **Personalized Request Matching**: Direct patient blood requests delivered straight to eligible donors based on blood group compatibility.
- **Donation History Log**: Complete self-service record tracking donation dates, units contributed, medical centers, and automated eligibility countdowns.

### 🔔 Real-Time Notification Center
- **Persistent Notification Store**: Database-backed notification history categorized by event type (Emergency, Ambulance, Milestone, General).
- **In-App Notification Bell & Dropdown**: Interactive navbar bell with real-time unread counter badge, category badges, relative time tags, and one-click mark-as-read.
- **SignalR Real-Time Alerts**: Instant, bidirectional push notifications delivered across client browsers when emergency blood or ambulance requests are submitted.
- **Zero-Dependency Emergency Audio Chimes**: Dual-tone emergency alerts synthesized via the browser's native Web Audio API (`880Hz -> 1046.5Hz`), functioning reliably without external audio files.

### 🛡️ Administrative Control Center
- **Role-Based Cookie Authentication**: Hardened security using `[AdminAuthorize]` and `[DonorAuthorize]` with role claims.
- **Full CRUD Management**: Comprehensive administration of Hospitals, Blood Banks, Ambulance Fleets, Oxygen Suppliers, CMS Pages, and Inquiries.
- **Audit Trail Logging**: Immutable audit logging tracking administrative mutations, donor status updates, and fleet dispatch actions.
- **Administrative Medical & Donation CSV Export**: One-click CSV reporting for donations, registered donors, and blood requests for institutional audits.
- **Live Dispatcher Broadcast Channel**: Real-time push notifications sent to dedicated administrative dispatcher groups with patient contact and pickup coordinates.

---

## 🛠️ Technology Stack

- **Framework**: .NET 10.0 (ASP.NET Core MVC)
- **ORM**: Entity Framework Core 10.0
- **Database**: Microsoft SQL Server / LocalDB (`(localdb)\MSSQLLocalDB`)
- **Real-Time Communication**: ASP.NET Core SignalR
- **Security & Cryptography**: BCrypt.Net-Next (adaptive hashing), Cryptographic 128-bit Tokens
- **QR Code Engine**: QRCoder 1.8.1
- **Mapping Engine**: Leaflet.js & OpenStreetMap
- **Architecture**: 4-Layer Clean Architecture (`Model` → `Repository` → `Service` → `Web`)
- **Testing**: xUnit test suite with relational in-memory database execution

---

## 🔒 Security Hardening

- **Cookie Authentication with Roles**: Cookie-based authentication with `Roles.Admin` and `Roles.Donor`. Secure, HttpOnly, and SameSite protection.
- **Built-in Rate Limiting**: Fixed-window partitioned rate limiting per client IP (10 requests/min for authentication, 15 requests/min for emergency submissions).
- **Anti-CSRF Protection**: All state-modifying endpoints strictly enforce HTTP POST with cryptographic anti-forgery tokens.
- **Security Response Headers**: Automated middleware applying `X-Frame-Options: SAMEORIGIN`, `X-Content-Type-Options: nosniff`, `Referrer-Policy: strict-origin-when-cross-origin`, and `Permissions-Policy`.
- **Donor Privacy Shield**: Contact details masked for unauthenticated visitors (`Sign in to view`), protecting donors from automated scraping.
- **ID Enumeration Prevention**: Public tracking routes for ambulance dispatches and blood requests rely strictly on 128-bit unguessable tokens rather than sequential integer IDs.

---

## 🚀 Getting Started

### Prerequisites
1. [.NET 10 SDK](https://dotnet.microsoft.com/download)
2. Microsoft SQL Server LocalDB or full SQL Server instance

### Setup & Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/aminaminul/BloodBankManagementSystem.git
   cd BloodBankManagementSystem
   ```

2. **Database Configuration**:
   Verify connection string in `BBDMS.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BloodBankMSDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. **Apply Database Migrations**:
   ```bash
   dotnet ef database update --project BBDMS.Repository --startup-project BBDMS.Web
   ```

4. **Build the Solution**:
   ```bash
   dotnet build BBDMS.sln
   ```

5. **Run the Test Suite**:
   ```bash
   dotnet test BBDMS.Tests
   ```

6. **Start the Web Application**:
   ```bash
   dotnet run --project BBDMS.Web
   ```
   Access the portal at `http://localhost:5148`.

---

## 🔑 Default Credentials (Development Seeding)

| Role | Username / Email | Password | Access URL |
|---|---|---|---|
| **Administrator** | `admin` | `admin` | `/Admin` |
| **Demo Donor 1** | `tanvir@example.com` | `donor` | `/Account/Login` |
| **Demo Donor 2** | `nusrat@example.com` | `donor` | `/Account/Login` |

*(Note: Passwords are automatically hashed using BCrypt upon application startup. In production environments, initial administrative credentials are provisioned securely via `Seed:AdminUserName` and `Seed:AdminPassword` configuration settings.)*

---

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.