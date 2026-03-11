# CLAUDE.md - Equipment Borrowing System

## Project Overview
Equipment Borrowing System - an internal web application for managing equipment lending/borrowing workflow.
Built with ASP.NET Core 7.0 MVC, Entity Framework Core, SQL Server, deployed on IIS via GitHub Actions.

## Tech Stack
- **Framework:** ASP.NET Core 7.0 MVC (C#)
- **ORM:** Entity Framework Core 7.0.17
- **Database:** SQL Server (remote: 192.168.10.160:1433, DB: Employee_Test)
- **Frontend:** Bootstrap 5.2, jQuery 3.6, Chart.js 2.9
- **Background Jobs:** Custom Task.Run loop (Hangfire installed but unused)
- **Email:** Gmail SMTP via IEmailSender
- **Deployment:** Self-hosted GitHub Actions runner -> IIS on 192.168.10.160

## Solution Structure
```
EquipmentBorrowingSystem/
├── .github/workflows/deploy.yml          # CI/CD pipeline
├── EquipmentBorrowingSystem/             # Main project
│   ├── Controllers/                      # 10 MVC controllers
│   ├── Data/EmployeeContext.cs           # EF Core DbContext
│   ├── EMail/                            # IEmailSender + EmailSender
│   ├── Migrations/                       # 9 EF migrations
│   ├── Models/                           # 15 entity models
│   ├── OverdueNotificationService/       # Background overdue checker
│   ├── ViewModel/                        # 6 view models
│   ├── Views/                            # Razor views (9 folders, 30+ files)
│   ├── wwwroot/                          # Static assets (CSS, JS, images)
│   └── Program.cs                        # App entry point & config
└── EquipmentBorrowingSystem.sln
```

## Key Controllers
| Controller | Purpose |
|-----------|---------|
| BorrowEquipment1Controller (835 lines) | Equipment browsing, application flow, member auth |
| Manage_CreateApplicationCompletedController (620 lines) | Manager approves/rejects applications |
| Manage_All_EquipmentController (495 lines) | Equipment CRUD management |
| SendEmail_OverdueNotificationController (445 lines) | Overdue email notifications |
| EquipmentReturnController (326 lines) | Equipment return process |
| ApplicationCompletedController (320 lines) | Completed application records |
| Count_Borrowing_TimesController (162 lines) | Borrowing statistics |
| ManagerController (105 lines) | Manager login/logout |

## Business Flow
1. Member registers/logs in
2. Browses available equipment catalog
3. Submits borrowing application (with borrow/return dates)
4. Manager reviews and approves/rejects
5. On borrow date: background service auto-updates status to "Borrowing"
6. On return: manager confirms equipment received
7. If overdue: auto-sends email notification

## Key Conventions
- **Language:** Code in English, UI and comments in Traditional Chinese (繁體中文)
- **Naming:** Controllers use snake_case (e.g., `Manage_All_Equipment`), C# properties use PascalCase
- **Status values:** String-based: "Not_borrowed_yet", "Borrowing", "Overdue", "Returned"
- **Boolean fields:** Stored as string "True"/"False" (not actual bool)
- **Quantity fields:** Stored as string, parsed with int.Parse() when needed
- **Auth:** Cookie-based, SharedData.WelcomeMessage static var for login state
- **Default route:** `{controller=BorrowEquipment1}/{action=Browse_equipment}/{id?}`

## Build & Run
```bash
# Restore packages
dotnet restore

# Run in development
dotnet run --project EquipmentBorrowingSystem

# Publish for deployment
dotnet publish -c Release -o ./publish
```

## Database
- Connection string in appsettings.json (key: "EmployeeContext")
- Migrations in /Migrations folder
- Apply migrations: `dotnet ef database update`
- Key tables: Equipment, Equipment_Details, Application, Application_Completed, Application_Details, tMember, tManager

## Known Technical Debt
1. **Security:** Credentials in appsettings.json committed to git; UseAuthentication() is commented out
2. **Background service:** Uses Task.Run+while(true) instead of IHostedService/BackgroundService
3. **Data types:** Quantities as string instead of int; booleans as "True"/"False" strings
4. **Architecture:** Business logic in controllers, no service layer
5. **Status management:** Magic strings instead of enums
6. **.NET version:** .NET 7 is EOL, should upgrade to .NET 8 LTS
7. **SharedData.WelcomeMessage:** Static variable for auth state, unsafe in multi-user scenarios

## Deployment
- **CI/CD:** GitHub Actions on push to main
- **Target:** IIS on 192.168.10.160
- **AppPool:** "EquipmentBorrowingSystem"
- **IIS Path:** C:\inetpub\wwwroot\EquipmentBorrowingSystem
- **Process:** Stop IIS -> robocopy files -> Start IIS
