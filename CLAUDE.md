# CLAUDE.md

Instructions for Claude (and any other AI assistant) working in this repository. **Read this file first in every session.**

## 1. What this project is

**RestoPOS**: one licensed system for restaurants, cafes, bakeries and retail shops. Built by two people: the lead developer (owner) and Claude.

Three ways of use from one code base:

| Way of use | What runs | Database |
|---|---|---|
| **Desktop, standalone** | The MAUI app runs the whole application itself on one PC | SQL Server or PostgreSQL on the same PC |
| **Web, cloud** | `RestaurantSaaS.Web` hosted by us (many businesses = tenants) | SQL Server or PostgreSQL |
| **Web, on-premise server** | The same Web project as a Windows Service in the restaurant (LAN) | SQL Server or PostgreSQL |

The desktop app can also run **Connected** (screens only, calls the Web API). The REST API (`/api/v1`) also serves the **mobile app that will be built later**.

**Fixed decisions (do not change silently):** only **SQL Server and PostgreSQL** (no SQLite anywhere); no MediatR (small built-in dispatcher); offline sync is not in Release 1.0; the API lives inside the Web project; all 236 tables exist as entity classes; EF Core code first.

## 2. Working agreements with the owner

- The owner writes **Roman Urdu mixed with English**. Reply in simple Roman Urdu (technical words stay English). Be short and clear. Code, comments, commit messages and documents are in **English with simple words**.
- Build **one module at a time**, in the order of the Development Flow. Start from the module's "done when" check.
- When the owner already has the project, give **only the changed and new files** (a patch zip with the same folder paths, plus a list). Do not resend the whole project unless asked.
- **Unit tests are on hold.** Do not write new tests unless the owner asks. Keep the existing tests compiling when you change a signature.
- **Never say code was compiled, run or tested if it was not.** The environment may have no .NET SDK. Say so, and ask for build errors.
- If a decision must change, say it clearly and update the documents (SRS, Database Schema, Architecture, FRS, Module Status, README).
- When a module is finished: update the FRS statuses, the Module Status Report and the README.
- Before writing code, **verify names against the real code** (entity properties, enum members, method signatures). Most build errors so far came from guessed names or overloads.

## 3. Commands

```powershell
dotnet build RestaurantSaaS.slnx
dotnet test
dotnet run --project src/RestaurantSaaS.Web          # web, https://localhost:7080
# Desktop: open in Visual Studio, set RestaurantSaaS.Desktop as startup, run on "Windows Machine"
```

Migrations (code first, one project per database). In Package Manager Console:

```powershell
$env:Database__Provider = "SqlServer"                 # or "PostgreSql"
Add-Migration <Name> -Project RestaurantSaaS.Migrations.SqlServer -StartupProject RestaurantSaaS.Web -OutputDir Migrations
Remove-Migration  -Project RestaurantSaaS.Migrations.SqlServer -StartupProject RestaurantSaaS.Web -Force
```

Or `scripts/add-migration.ps1 -Provider <SqlServer|PostgreSql> -Name <Name>`. A schema change needs **one migration per supported database**. **Until the first client goes live**, delete the migration, drop the databases (`RestaurantSaaS` for web, `RestoPOS_Desktop` for desktop) and create `Initial` again.

Development sign-ins (Development and Debug builds only): SuperAdmin `superadmin` / `ChangeMe!123` (empty business code); demo Owner business code `DEMO`, `owner` / `Demo@12345`. Reset codes are written to the log (`Verification:LogCodes`).

## 4. Solution structure and the dependency rule

```
src/RestaurantSaaS.Contracts        DTOs, paging, UI service interfaces, ServiceException   (no dependencies)
src/RestaurantSaaS.Domain           236 entities, enums, Permissions catalog, SystemRoles   (no dependencies)
src/RestaurantSaaS.Application      requests + handlers, validators, authorization, Local services
src/RestaurantSaaS.Infrastructure   AppDbContext, configurations, interceptors, hasher, seeding, license key/file/fingerprint services
src/RestaurantSaaS.Migrations.SqlServer / .PostgreSql   migrations of one provider each
src/RestaurantSaaS.Shared.UI        Razor class library: ALL screens, components, CSS, AuthSession (web and desktop)
src/RestaurantSaaS.Web              server: Blazor host + REST API + SignalR + hourly license job
src/RestaurantSaaS.Desktop          MAUI Blazor Hybrid: Standalone or Connected
tests/ UnitTests, IntegrationTests
```

Dependencies point inward only. `Shared.UI` references Contracts (and Domain for permission keys only), **never EF Core or Application**.

The UI calls **service interfaces** from `Contracts` (for example `IUserService`). Two implementations: **Local** (in-process, in `Application/Services`, used by the Web pages and the standalone desktop app) and **Remote** (HTTP, in `Shared.UI/Services/Remote`, used by the connected desktop app and later the mobile app).

## 5. Patterns you must follow

**Use case = one request + one handler (+ validator), no business logic anywhere else.**

```csharp
[RequirePermission(Permissions.UserView)]
public sealed record GetUsersPageQuery(PageRequest Page) : IRequest<PagedResult<UserListItemDto>>;
```

- **Default deny.** A request needs a signed-in user unless it has `[AllowAnonymousRequest]`. `[RequirePermission]` adds a permission. The SuperAdmin passes all permission checks. Pages use `[Authorize(Policy = Permissions.X)]` (one policy per permission).
- Expected failures throw `AppException` types (`RequestRejectedException`, `UnauthorizedException`, `ForbiddenException`, `LicenseException` = HTTP 402) with a stable code. The UI only sees `ServiceException`.
- **Audit** sensitive actions with `AuditTrail.Add(...)` before `SaveChangesAsync` (never put a password, PIN, token or license key in it).
- Any table is reached with `db.Set<TEntity>()` (or add a typed `DbSet` to `IApplicationDbContext`).
- Paging: offset (`PageRequest`/`PagedResult`, default 25, max 100, whitelisted sort columns, `Id` last) for back-office lists; cursor (date column + `Id`, never `Id` alone because SQL Server does not order Guid v7 by time) for large growing lists; none for live boards.
- Time from `IClock`. Money is `decimal`. List queries use `AsNoTracking`, projection and paging.
- **Selling requests** (order, payment, bill: M09, M11) must carry `[RequireActiveLicense]`.

**Data layer (applied automatically by `AppDbContext` and `AuditingInterceptor`):** Guid v7 ids created by the app; audit columns; Delete becomes soft delete; other tenants' rows are hidden (no tenant = no tenant rows); a row of another tenant cannot be saved; unique indexes ignore deleted rows and null key parts; concurrency token (SQL Server `rowversion`, PostgreSQL `xmin`); all foreign keys are Restrict. Sign-in, token refresh and platform work read with `IgnoreQueryFilters()` and an explicit condition.

## 6. Checklist: add a feature (one module step)

1. Entity already exists? (all 236 do). Check its columns against the module's needs; change it only if needed, then migrate.
2. `Contracts`: DTOs and the service interface (add a method, or a new `I...Service`).
3. `Application/Features/<Area>`: request, handler, validator; permission attribute; audit call.
4. `Application/Services`: Local service method; register in `AddLocalClientServices`.
5. `Web/Endpoints/Modules`: thin endpoint; map it in `ApiEndpoints`.
6. `Shared.UI/Services/Remote`: Remote service method; register in `AddRemoteClientServices`.
7. `Shared.UI/Pages`: the screen (`[Authorize(Policy = ...)]`, catch `ServiceException`, show field errors); add a `NavMenu` item with its permission.
8. New permission: add it to `Permissions.cs` (catalog) and to `SystemRoles` if a system role gets it. The Owner role gets every new permission automatically at start-up.
9. Schema change: migration for **both** databases. Update the Database Schema document.
10. Update the FRS statuses, the Module Status Report and the README.

## 7. UI rules

Bootstrap 5.3 and Bootstrap Icons only (no CDN at run time). Light and dark theme through CSS variables. Screens work on tablets (touch size) and on desktop. Every list: search, sort, `Pager`. Every action button shows a busy state. Never store tokens in `localStorage` (use `ISecureStore`). After sign-in always go through `ReturnUrlHelper.Resolve`. No raw HTML from users.

## 8. Security rules

Passwords and PINs only as PBKDF2 hashes (`IPasswordHasher`). Every sign-in failure gives the same message. 5 wrong attempts lock for 15 minutes. Never log secrets, tokens, passwords, PINs, license keys or card data. Secrets (`Jwt:SigningKey`, `License:KeyPepper`, `License:SigningPrivateKeyPem`, passwords) come from environment variables in production, never from the repository. The server checks every permission itself; the UI only hides things.

## 9. Environment gotchas already met

- The **standalone desktop** must register what the web registers itself: `ITokenService` (`OpaqueTokenService`), `ICurrentUser` and `ITenantProvider` (`AccessorRequestContext`), `IDeploymentInfo`. A missing service shows as "Unable to resolve service ...".
- A navigation property must not have the same name as a column (for example `TaxRate` and `TaxRateId`).
- `LocalExecutor` resolves `ISessionAccessor` late, to avoid a circular dependency with `AuthSession`.
- The web renders with `prerender: false`, so JS interop works when the first screen starts.
- `@attribute [Authorize]` is in `Shared.UI/_Imports.razor`: every page needs a signed-in user unless it has `[AllowAnonymous]`.
- `dotnet ef` builds the host but does not run it; database start-up is a hosted service for that reason.
- The desktop app does not remember the sign-in (`remember_signin` preference is false by default).
- `HttpClient.PostAsJsonAsync` has no overload that takes a `Type`; use `JsonContent.Create(body, body.GetType())` with `PostAsync`.
- A `.csproj` must have only XML tags inside `<PropertyGroup>`; a stray text line gives MSB4 "`<#text>` is unrecognized".
- Licensing services (`LicenseBehavior`) resolve `ILicenseGate` lazily, so tests and cloud requests without the attribute never need the license services.

## 10. Documents (read before a module)

| Document | Use it for |
|---|---|
| Development Flow and Module Build Sequence (Word) | The order of modules, their steps and "done when" checks |
| Functional Requirements Specification (FRS) | Every function of every module (FR-Mxx-nn) |
| Module Status and Progress Report | What is done and what is next |
| SRS | Business rules and numbers |
| Database Schema Design | All 236 tables |
| Technical Architecture | Decisions (ADR), flows |
| `docs/M04-Licensing.md` | How licensing works, settings, and manual checks |
| UI/UX Specification, Test Plan, Deployment Guide, Project Plan | As named |

Links are in `docs/DOCUMENTS.md`. Where a later section of a document says it replaces an earlier one, the later section wins.

## 11. Licensing (M04) in short

Read `docs/M04-Licensing.md` before touching anything license related.

- **Keys:** `XXXXX-XXXXX-XXXXX-XXXXX` (16 random + 4 checksum). Only an HMAC hash (with `License:KeyPepper`) is stored. The plain key is shown once.
- **License file:** `license.lic` (signed JSON, ECDSA P-256) in `%LocalAppData%\RestoPOS`. The License Server holds the private key; every app trusts only public keys (`License:TrustedKeys`).
- **Desktop and on-premise:** activation screen (`/activate`) first, then first-run setup. A 14-day trial needs name and phone (one trial per PC, phone, email). Start-up check: signature, PC fingerprint (3 of 4 parts), sequence against the last `LicenseRecord`, record hash chain, clock mark.
- **Cloud:** no file. A key creates a `Subscription`; the 14-day sign-up trial runs until then.
- **States:** NotActivated, Trial, Active, Expiring (14 days), Grace (7 days, paid only), Expired, Tampered, ClockError, Mismatch, Revoked, NotRequired.
- **Lock:** a desktop or on-premise PC that is NotActivated (never activated, or deactivated) refuses every request that is not anonymous (`LicenseBehavior`), and the screens go to `/activate` (`LicenseLock`).
- **Deactivate:** permission `License.Deactivate` (Owner by default). Frees the activation on the License Server and removes the file. The same key can be used again; a removed trial cannot be restarted. Changing a paid key frees the old one automatically. SuperAdmin can free all PCs of a key ("Free PCs").
- **Permissions:** `License.View`, `License.Activate`, `License.Deactivate`, `License.Generate` (platform only).
- **Not yet done (by plan):** offline activation, transfer, revoke screen, reminders, OS-protected secret, repair after restore (M17, M37); plans and feature switches (M34); SuperAdmin portal (M35).

## 12. Where we are

Modules M00 to M04 are written (**not yet built and tested**). The M04 patch has to be built first; fix build errors in small steps, then run the manual checks in `docs/M04-Licensing.md`. **Next module: M05 Business Settings** (FR-M05-01 to FR-M05-07), then M06 Menu, M07 Tables, M08 Customers to finish Sprint S3. See the Module Status Report for the full list.
