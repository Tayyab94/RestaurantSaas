# RestoPOS: Restaurant and Business Management SaaS

One complete, professional system for restaurants, cafes, bakeries and similar businesses: from a single shop with one
computer to a multi-branch chain. Version `0.3.0`.

Done so far: **M00 Foundation**, the **complete schema as code (all 236 entity classes)**, and the first slice of
**M01 Registration, M02 Login, M03 Users and Roles**. Documents: SRS v2.6, Database Schema v2.7, Technical Architecture v1.4,
UI/UX v1.1, Project Plan v2.2, Test Plan v1.6, Deployment Guide v1.3, Development Flow v1.5.

---

## 1. The three ways the product is used

| Product | What runs where | Database | For whom |
|---|---|---|---|
| **Desktop (standalone)** | The MAUI desktop app on the client's PC runs the whole application itself. No server, no hosting | **SQL Server or PostgreSQL**, installed on the same PC | Clients who only want a desktop app |
| **Web (cloud)** | The Web project is hosted by us (Azure, AWS, a hosting provider). Browsers, tablets and phones connect | **SQL Server or PostgreSQL** on the server | Chains and owners who want to work from anywhere |
| **Web (on-premise server)** | The same Web project runs as a Windows Service on a PC in the restaurant; other devices connect over the LAN, no internet needed | **SQL Server or PostgreSQL** on that PC | Restaurants with several devices (kitchen screen, waiters' tablets) |

The desktop app also has a **Connected** mode: it shows the same screens but talks to a Web server over the API (LAN or cloud),
instead of using its own database.

**The API** (`/api/v1`, inside the Web project) serves the web UI, the desktop app in Connected mode, and **the mobile app that will be built later**.

**Databases: only two, SQL Server and PostgreSQL**, chosen by configuration. SQLite is not used anywhere. Cloud is only a place to run
the server; it does not decide the database (Azure and AWS offer both engines).

---

## 2. What is a tenant? Users, roles, SuperAdmin

- **Tenant** = one customer business ("Mario's Pizza"). Many tenants share one running web system; each tenant's data is completely separate (every tenant table has a `TenantId`). A desktop installation has exactly one tenant.
- **Branch** = one outlet of a tenant. **User** = a person who signs in. **Role** = a named set of **permissions** (Owner, Manager, Cashier, Waiter, Kitchen are created for every business). **SuperAdmin** = the platform owner (us), outside every tenant.

**The start sequence**
1. First start: migrations are applied, the permission catalog is saved and the **default SuperAdmin** is created (only when `Seed:SuperAdminPassword` is set; it must change its password at first sign-in).
2. **Registration** (`/register`) creates the business, main branch, five roles and the Owner, and shows the **business code**.
3. **Sign-in** (`/login`): business code + username + password (SuperAdmin leaves the code empty).
4. The Owner adds users (`/admin/users`) and gives each a role. Next module: **M04 Licensing core**.

Development sign-ins (Development and Debug builds only): SuperAdmin `superadmin` / `ChangeMe!123` (empty business code); demo Owner business code `DEMO`, `owner` / `Demo@12345`.

---

## 3. Technology

| Layer | Choice |
|---|---|
| Runtime | .NET 10, C# (latest) |
| Web UI | ASP.NET Core Blazor Web App (interactive server rendering first), Bootstrap 5.3, Bootstrap Icons |
| Desktop | .NET MAUI Blazor Hybrid (Windows): the same screens, Standalone or Connected mode |
| API | Minimal APIs inside the Web project (`/api/v1`), OpenAPI, ProblemDetails; used by web, desktop (Connected) and the future mobile app |
| Real time | SignalR hub in the Web project |
| Data | Entity Framework Core 10, code first; SQL Server or PostgreSQL |
| Security | PBKDF2-SHA256 passwords; JWT access token (15 min) + refresh token (14 days, stored hashed); default-deny authorization on the server |
| Application layer | Built-in request dispatcher with behaviors (authorization, validation), FluentValidation |
| Logging, packages | Serilog; Central Package Management |

---

## 4. Solution structure

```
RestaurantSaaS.slnx
src/
  RestaurantSaaS.Contracts            DTOs, paging contracts, UI service interfaces (no dependencies)
  RestaurantSaaS.Domain               ALL entity classes (236), enums, permission catalog, system roles (no dependencies)
  RestaurantSaaS.Application          use cases (requests + handlers), validators, authorization, abstractions
  RestaurantSaaS.Infrastructure       EF Core DbContext, table configurations, interceptors, password hasher, seeding
  RestaurantSaaS.Migrations.SqlServer      EF migrations for SQL Server
  RestaurantSaaS.Migrations.PostgreSql     EF migrations for PostgreSQL
  RestaurantSaaS.Shared.UI            Razor Class Library: layouts, components, pages, CSS, sign-in session
  RestaurantSaaS.Web                  Server: Blazor host + REST API + SignalR (cloud or on-premise)
  RestaurantSaaS.Desktop              MAUI Blazor Hybrid app: Standalone (own database) or Connected (API)
tests/  UnitTests, IntegrationTests
scripts/  add-migration.ps1, update-database.ps1
```

| Host | Implementation of the UI service interfaces |
|---|---|
| Web server pages | **Local**: in-process calls into the application layer |
| Desktop, Standalone mode | **Local**: the application runs inside the app, with its own database |
| Desktop, Connected mode (and the future mobile app) | **Remote**: HTTP to the Web API |

Dependency rule: inward only. `Shared.UI` references Contracts (and Domain for permission keys only), never EF Core.

---

## 5. The complete schema as code

All **236 tables** of the Database Schema document exist as classes: 8 hand-written identity classes with behavior
(Tenant, Branch, User, Role, Permission, RolePermission, UserRole, RefreshToken) and **228 more**, grouped by module folder in `Domain/Entities/<Module>/`.
Each has its enums (`Domain/Enums/<Module>Enums.cs`, 171 enums), its EF configuration
(`Infrastructure/Persistence/Configurations/Generated/<Module>Configurations.cs`: lengths, decimal precision, foreign keys, unique indexes)
and a `DbSet` (`AppDbContext.DbSets.cs`).

| Module | Tables | Entities |
|---|---|---|
| Platform | 31 | Plan, Feature, PlanFeature, BusinessTypeTemplate, TemplateFeature, TenantFeature, Subscription, SubscriptionInvoice, SubscriptionPayment, Setting, Device, Printer, PrinterRoute, ReceiptTemplate, TaxRate, NumberSequence, PaymentMethod, IntegrationSetting, ApiKey, WebhookSubscription, Currency, PlatformSetting, AppRelease, ImpersonationSession, Area, OnboardingProgress, DataExport, RestoreLog, AppUpdateLog, InstallationInfo, Translation |
| Licensing | 15 | LicenseSigningKey, LicenseBatch, License, LicenseActivation, LicenseTransfer, TrialRegistry, LicenseEvent, LicenseRecord, LicenseClockMark, LicenseCheckLog, LicenseRequest, LicenseReminder, LicenseAttempt, LicenseChange, LegacyLicense |
| Identity | 11 | UserBranch, UserPermissionOverride, ApprovalLimit, VerificationCode, UserTwoFactor, UserPreference, UserSession, LoginHistory, AuditLog, ErrorLog, ApprovalLog |
| Menu | 15 | MenuCategory, Station, MenuItem, ItemVariant, ModifierGroup, Modifier, ItemModifierGroup, BranchMenuItem, Deal, DealItem, ItemBarcode, MenuItemPriceHistory, Allergen, MenuItemAllergen, RawMaterialAllergen |
| Floors | 5 | Floor, RestaurantTable, Reservation, WaitingListEntry, QrOrderSession |
| Customers | 20 | CustomerGroup, Customer, CustomerAddress, CustomerContact, CustomerLedgerEntry, CustomerAdvance, LoyaltyProgram, LoyaltyTransaction, GiftCard, GiftCardTransaction, Promotion, Coupon, CouponRedemption, Feedback, PriceList, PriceListItem, ConsolidatedInvoice, ConsolidatedInvoiceLine, CustomerMergeLog, ImportJob |
| Orders | 16 | Order, OrderTable, OrderItem, OrderItemModifier, OrderDiscount, OrderStatusHistory, OrderNote, OrderTransfer, DiscountPreset, ReasonCode, RemakeRecord, ItemAvailabilityLog, KdsScreen, KitchenTicket, KitchenTicketItem, KitchenPerformanceDaily |
| Billing | 9 | Invoice, InvoiceItem, InvoiceTax, Payment, Tip, PrintLog, PrintJob, FbrSubmission, DailySalesSummary |
| Cash | 4 | Shift, CashMovement, CashDrawerLog, DayClosing |
| Delivery | 6 | DeliveryArea, Rider, Delivery, DeliveryStatusHistory, RiderSettlement, RiderEarning |
| Returns | 3 | ReturnReason, ReturnOrder, ReturnItem |
| Inventory | 31 | Unit, UnitConversion, RawMaterialCategory, RawMaterial, Recipe, RecipeItem, RecipeStep, StockLocation, BranchStock, StockBatch, StockMovement, StockAdjustment, StockAdjustmentItem, WastageRecord, StockTransfer, StockTransferItem, StockCount, StockCountItem, ProductionRun, PackagingRule, StockRequisition, StockRequisitionItem, VarianceAnalysis, ReorderSuggestion, RecipeCostSnapshot, PrepList, PrepListItem, ProductionPlan, ProductionPlanItem, ProductionPlanDemand, PrepLabel |
| Purchasing | 14 | Supplier, SupplierContact, SupplierItem, SupplierItemPriceHistory, SupplierAdvance, SupplierEvaluation, SupplierLedgerEntry, SupplierPayment, PurchaseOrder, PurchaseOrderItem, GoodsReceipt, GoodsReceiptItem, PurchaseReturn, PurchaseReturnItem |
| Staff | 21 | ExpenseCategory, Expense, Employee, WorkShift, EmployeeSchedule, Attendance, LeaveType, LeaveBalance, LeaveRequest, Holiday, PayComponent, EmployeePayComponent, PayrollRun, Payroll, PayrollItem, PayAdjustment, TipPool, TipPoolShare, EmployeeLoan, LoanInstallment, LoanTransaction |
| Accounting | 9 | ChartOfAccount, AccountMapping, AccountingPeriod, JournalEntry, JournalLine, BankAccount, BankTransaction, Cheque, AccountingExport |
| FoodSafety | 7 | Equipment, FoodSafetyChecklist, ChecklistItem, ChecklistRun, ChecklistRunItem, TemperatureLog, FoodSafetyIncident |
| System | 11 | Notification, NotificationRecipient, Attachment, MessageLog, MessageTemplate, BackupLog, OwnerReportSchedule, PushToken, IntegrationLog, ExternalMenuMapping, WebhookDelivery |

How the entities are written:
- Base class by scope: `BaseEntity` (platform), `TenantEntity` (adds `TenantId`), `BranchEntity` (adds `BranchId`). Tables owned by a tenant **or** the platform implement `IMayHaveTenant`.
- Plain properties with public setters, and one navigation property per foreign key (reference side). Behavior (factory methods, rules) is added to an entity when its module is built, as it was for the identity classes.
- All foreign keys restrict deletes (rows are soft-deleted). Decimal precision: money (18,2), quantities (18,3), percentages (9,4).
- Unique indexes ignore soft-deleted rows and null key parts.
- `SchemaModelTests` fails if any entity is missing from the model, a table lacks the standard columns, or a foreign key is not Restrict.

**Adding a table**: add the class in `Domain/Entities/<Module>`, a configuration, a `DbSet`, then a migration for each database you support.
**Using a table in a handler**: `db.Set<Order>()` (any table), or add a typed property to `IApplicationDbContext`.

---

## 6. Getting started in Visual Studio

Prerequisites: Visual Studio 2026 with *ASP.NET and web development*, *.NET Multi-platform App UI development* and *Data storage and processing* (LocalDB), or PostgreSQL installed.

**Web (cloud or on-premise server)**
1. Open `RestaurantSaaS.slnx`. If styles are missing, right-click `Shared.UI/libman.json` > *Restore Client-Side Libraries*.
2. Create the first migration: `./scripts/add-migration.ps1 -Provider SqlServer -Name Initial` (or `-Provider PostgreSql`).
3. Set `RestaurantSaaS.Web` as the startup project, press **F5**, open `https://localhost:7080`.

**Desktop (standalone)**
1. Make sure the migration above exists (the desktop app uses the same migration projects).
2. Set `RestaurantSaaS.Desktop` as the startup project and run on *Windows Machine*. It starts in **Standalone** mode with SQL Server LocalDB (`RestoPOS_Desktop` database) and the same development sign-ins.
3. To use PostgreSQL, or the Connected mode, change the settings (MAUI Preferences keys `mode`, `db_provider`, `db_connection`, `server_url`). The installer or a first-run screen will write them in module M38.

**Using PostgreSQL**: set `Database:Provider` to `PostgreSql` and a PostgreSQL connection string, then `./scripts/add-migration.ps1 -Provider PostgreSql -Name Initial`.

**About migrations before the first client**: the schema is large and will still change while modules are built. Until the first real client goes live, you may delete the `Migrations` folder, drop the database and create `Initial` again. After go-live, every change is a new migration.

---

## 7. Configuration (Web: `appsettings*.json`)

| Key | Meaning |
|---|---|
| `Database:Provider` / `Database:ConnectionString` | `SqlServer` (default) or `PostgreSql`, and its connection string |
| `Deployment:Mode` | `Cloud` (default) or `OnPremise` |
| `Jwt:SigningKey` | Token secret, at least 32 characters. **Required.** In production use the environment variable `Jwt__SigningKey` |
| `Seed:SuperAdminPassword` | Creates the default SuperAdmin at first start. Empty = none is created |
| `Seed:DemoData`, `Seed:DemoOwnerPassword` | Development only: creates the DEMO business |

---

## 8. Data layer rules

Every entity has `Id` (Guid v7, created by the application), `CreatedAt/By`, `UpdatedAt/By`, `IsDeleted`, `DeletedAt/By`, `RowVersion`.
`AppDbContext` and `AuditingInterceptor` apply them: audit columns on every save, **Delete becomes soft delete**, other tenants' rows are hidden (and no tenant means no tenant rows), a row of another tenant cannot be saved, concurrency token (SQL Server `rowversion`, PostgreSQL `xmin`), foreign keys to Tenant and Branch.
Cursor sorting must use a date column plus `Id` (SQL Server does not order Guid v7 values by time).

## 9. Application layer

One request = one class + one handler (+ validator). **Default deny**: a request needs a signed-in user unless it has `[AllowAnonymousRequest]`; `[RequirePermission]` adds the permission check. Expected failures are `AppException` types mapped to ProblemDetails; the UI sees one error type, `ServiceException`.

## 10. Pagination

Offset (`PageRequest`/`PagedResult`) for back-office lists (default 25, max 100); cursor (`CursorRequest`/`CursorResult`) for large time-ordered lists; none for live boards. Reference: `GetUsersPageQuery`.

## 11. Coding standards

`.editorconfig` is the law; nullable on; async with `CancellationToken`; no business logic in components or endpoints; UI uses DTOs, never entities; time from `IClock`; money is `decimal`; list queries use `AsNoTracking` and paging; never log secrets; every feature has tests and a review (AI-written code is reviewed and run before merging).

## 12. Testing

`dotnet test`: paging, domain rules, password hasher, authorization, registration and sign-in (lock-out), tenant isolation, soft delete, and the **whole-schema model tests**. Next: sign-in tests on real PostgreSQL and SQL Server (Testcontainers).

---

## 13. Decisions

| # | Decision |
|---|---|
| 1 | Only **SQL Server and PostgreSQL**; no SQLite |
| 2 | The desktop app is a **real standalone product** (own database on the PC) and can also run Connected |
| 3 | The web hosts the API for web, desktop (Connected) and the future mobile app |
| 4 | Offline sync is deferred (not in Release 1.0) |
| 5 | Small built-in dispatcher instead of MediatR |
| 6 | The complete schema exists as entity classes; tables are used module by module |
| 7 | One migration project per database |
| 8 | Refresh tokens are not rotated |

## 14. Known limitations (read before the first build)

- **This code was written without a .NET 10 SDK to compile it.** Expect build errors on the first build; with 380 C# files a few typing mistakes are likely. Fix them in small steps; run `dotnet test` (the schema tests point at configuration mistakes). If the **Desktop** project does not load, create a *.NET MAUI Blazor Hybrid App* in Visual Studio and copy over `MauiProgram.cs`, `DesktopSettings.cs`, `MauiSecureStore.cs`, `Components/Routes.razor` and `wwwroot/index.html`.
- No migration is committed: create `Initial` for your database. The first migration creates all 236 tables (most stay empty until their module is built).
- Desktop Standalone: SQL Server LocalDB is for development; for clients the installer must install SQL Server Express or PostgreSQL (module M38). Standalone has no live push between devices (a single PC).
- Registration does not verify phone or email yet; there is no password reset or PIN login yet.
- New permissions added later are not copied automatically into existing tenants' roles (a sync step is planned).
- The dashboard shows demo data; the license card is a placeholder until M04.

## 15. Roadmap

Pilot week 10, Release 1.0 week 38, Release 1.1 week 42. Next module: **M04 Licensing core**, then M05 Business Settings.
