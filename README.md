# Chubb APAC Claims Management

A full-stack claims management application created for the Chubb APAC Engineering Fullstack Developer take-home assessment. The project combines an Angular claimant and staff interface with a .NET REST API, a SQL Server data store, and a status-driven claims workflow.

> **Assessment/demo project:** This repository documents the implementation found in the source. It is not a production-ready claims platform and does not provide real authentication or authorization.

## Problem statement and objectives

The application demonstrates how claimants can submit and follow a claim, and how claims staff can process it through a basic lifecycle. The implementation uses a layered .NET backend and a single Angular application with separate claimant-facing and staff-facing pages.

Objectives include:

- Persist claims and audit events in a relational database.
- Provide API operations for claim lookup and staff decisions.
- Apply status-transition rules in backend command handlers.
- Provide frontend validation, loading feedback, and error handling.
- Demonstrate MediatR/CQRS, Entity Framework Core, and FluentValidation.

## Key features

### Claimant

- Submit a motor or property claim with claimant contact, market, incident, and estimated amount details.
- Track a claim by its claim number and see a friendly not-found message for an unknown number.
- Search claims with the email address supplied at submission. This is a demo filter, not ownership verification.
- Provide additional information when a claim is in `InformationRequested`.

### Claims staff

- View and refresh the claims list.
- Assign a `Submitted` claim to an officer.
- Request further information for an `UnderReview` claim.
- Approve or reject claims in `UnderReview` or `InformationProvided`, with a decision reason and confirmation.
- Settle an `Approved` claim after confirmation.

### Dashboard

- Show claim totals by status, recent claims, active officer workloads, and estimated outstanding liability.
- Refresh dashboard values from the backend.

## Technology stack

| Technology | Use |
|---|---|
| Angular 21 and TypeScript | Standalone web UI for claimant and staff workflows; Signals hold component state. |
| Angular Forms and RxJS | Template-driven form handling and asynchronous HTTP requests. |
| ASP.NET Core on .NET 10 | REST API controllers and application host. |
| MediatR and CQRS | Routes API requests through application queries and commands. |
| FluentValidation | Validates create and staff-action commands before handlers run. |
| Entity Framework Core 10 | ORM and SQL Server persistence. |
| SQL Server | Relational storage for claims and claim audit entries. |

## Dependencies and versions

Versions below are resolved versions from `claims-ui/package-lock.json` for direct frontend dependencies, and `PackageReference` versions from the .NET project files. Transitive dependencies are not listed as direct dependencies.

### Frontend

| Direct runtime dependency | Resolved version |
|---|---:|
| `@angular/cdk` | 21.2.14 |
| `@angular/common` | 21.2.25 |
| `@angular/compiler` | 21.2.25 |
| `@angular/core` | 21.2.25 |
| `@angular/forms` | 21.2.25 |
| `@angular/material` | 21.2.14 |
| `@angular/platform-browser` | 21.2.25 |
| `@angular/router` | 21.2.25 |
| `rxjs` | 7.8.2 |
| `tslib` | 2.8.1 |

| Direct development dependency | Resolved version |
|---|---:|
| `@angular/build` | 21.2.28 |
| `@angular/cli` | 21.2.28 |
| `@angular/compiler-cli` | 21.2.25 |
| `jsdom` | 28.1.0 |
| `prettier` | 3.9.10 |
| `typescript` | 5.9.3 |
| `vitest` | 4.1.12 |

The package manifest declares `npm@10.9.4` as its package manager. It does not pin a Node.js version. The current workspace toolchain used for verification was Node.js 22.22.1 and npm 11.6.0.

### Backend

| Project | Direct package references |
|---|---|
| `Claims.Api` | `Microsoft.AspNetCore.OpenApi` 10.0.12; `Microsoft.EntityFrameworkCore.Design` 10.0.13; `Swashbuckle.AspNetCore` 10.3.1 |
| `Claims.Application` | `FluentValidation` 12.2.0; `FluentValidation.DependencyInjectionExtensions` 12.2.0; `MediatR` 12.5.0; `Microsoft.EntityFrameworkCore` 10.0.13 |
| `Claims.Infrastructure` | `Microsoft.EntityFrameworkCore` 10.0.13; `Microsoft.EntityFrameworkCore.Design` 10.0.13; `Microsoft.EntityFrameworkCore.SqlServer` 10.0.13 |
| `Claims.Domain` | No direct NuGet package references |

All backend projects target `net10.0`. The installed SDK used for verification was .NET SDK 10.0.401.

## Architecture

The solution is split into four .NET projects and one Angular project:

- **`Claims.Api`** — ASP.NET Core controllers, HTTP request/response mapping, CORS, Swagger in Development, and API exception handling.
- **`Claims.Application`** — MediatR commands and queries, handlers, DTOs, validators, and the reusable FluentValidation pipeline behavior.
- **`Claims.Domain`** — Claim and audit entities and the claim status/type enums.
- **`Claims.Infrastructure`** — EF Core `AppDbContext`, SQL Server provider configuration, and migrations.
- **`claims-ui`** — Standalone Angular application and `ClaimService` HTTP client.

## Claim lifecycle

The create operation starts a claim in `Submitted`. The backend command handlers enforce these transitions:

```text
Submitted
  └─ assign ─> UnderReview
                  ├─ request information ─> InformationRequested
                  │                            └─ provide information ─> InformationProvided
                  ├─ approve ─> Approved ─> settle ─> Settled
                  └─ reject ─> Rejected

InformationProvided
  ├─ approve ─> Approved
  └─ reject ─> Rejected
```

`UnderReview` and `InformationProvided` are the only statuses eligible for approval or rejection. `Approved` is the only status eligible for settlement.

## API reference

Routes are defined by `ClaimsController` (`/api/Claims`) and `DashboardController` (`/api/dashboard`).

| Method | Path | Purpose |
|---|---|---|
| `POST` | `/api/Claims` | Create a claim and its initial audit entry. |
| `GET` | `/api/Claims` | List claims. Optional query filters: `status`, `market`, `assignedOfficer`, and `claimantEmail`. |
| `GET` | `/api/Claims/{id}` | Retrieve a claim by its GUID. |
| `GET` | `/api/Claims/track/{claimNumber}` | Retrieve a claim by its exact claim number. |
| `POST` | `/api/Claims/{id}/assign` | Assign an eligible Submitted claim to an officer. |
| `POST` | `/api/Claims/{id}/request-information` | Request information for an UnderReview claim. |
| `POST` | `/api/Claims/{id}/provide-information` | Submit additional information for an InformationRequested claim. |
| `POST` | `/api/Claims/{id}/approve` | Approve an eligible claim with a decision reason. |
| `POST` | `/api/Claims/{id}/reject` | Reject an eligible claim with a decision reason. |
| `POST` | `/api/Claims/{id}/settle` | Settle an Approved claim. |
| `GET` | `/api/Claims/{id}/audit` | Retrieve audit events for a claim. |
| `GET` | `/api/dashboard` | Return claim status totals, workloads, recent claims, and liability. |

The list endpoint compares each supplied filter against the corresponding claim field. The claimant-email filter trims surrounding whitespace in the handler; whether equality ignores case depends on the configured SQL Server collation. It is not an ownership check.

## Validation and error handling

- `Claims.Application` registers its FluentValidation validators and the MediatR `ValidationBehavior<TRequest,TResponse>`. The behavior runs applicable validators before the request handler and stops the request on validation failures.
- `Claims.Api` converts FluentValidation failures to HTTP `400` `ValidationProblemDetails`, grouped by field.
- Command handlers enforce claim status transitions and return not-found or conflict responses for missing claims and invalid transitions.
- The Angular submit form and submit method validate required values, email format, date, claim type, amount, and field lengths. My Claims checks that a nonempty search value is a valid email.
- The UI uses loading and error states, disables active action controls, and has in-method guards for claim submission and several lookup/action flows.
- The frontend requires an estimated amount greater than zero, while the existing backend create validator accepts zero (`GreaterThanOrEqualTo(0)`). Direct API callers therefore have a less restrictive amount rule than the UI.

## Prerequisites and setup

### Requirements

- .NET 10 SDK (the projects target `net10.0`).
- Node.js compatible with Angular CLI 21. The repository does not pin a Node.js version.
- npm; `package.json` declares `npm@10.9.4`.
- A reachable SQL Server instance and a database account with permissions to create/update the schema and read/write claim data.

The API reads the connection string named `DefaultConnection`. Configure it locally through the `ConnectionStrings__DefaultConnection` environment variable or another ASP.NET Core configuration provider. Do not commit credentials. For example, in PowerShell, set a connection string for your own SQL Server and credentials in the same terminal used to run the API:

```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=<server>;Database=ClaimsManagementDb;User Id=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=True'
```

The repository contains an EF Core `InitialCreate` migration for `Claims` and `ClaimAudits`. No SQL setup script or automatic startup migration was found: the API does **not** apply migrations when it starts. Apply the migration before first use. If the EF Core CLI (`dotnet-ef`) is available, run this from the repository root:

```powershell
dotnet ef database update --project Claims.Infrastructure/Claims.Infrastructure.csproj --startup-project Claims.Api/Claims.Api.csproj --context AppDbContext
```

The `dotnet-ef` CLI is not declared as a repository-local tool. Install/use a compatible EF Core 10 CLI separately if it is not already available. No seed data or automatic database initialization is included.

### Run the backend

From the repository root:

```powershell
dotnet restore ClaimsManagement.slnx
dotnet run --project Claims.Api/Claims.Api.csproj --launch-profile http
```

The `http` launch profile uses `http://localhost:5192` and sets the Development environment. Swagger is configured at `http://localhost:5192/swagger` in Development. The `https` launch profile also configures `https://localhost:7237`.

### Run the frontend

In a separate terminal:

```powershell
cd claims-ui
npm ci
npm start
```

The Angular development server is configured for `http://localhost:4200`. `ClaimService` currently targets `http://localhost:5192/api`.

### Build and test commands

Frontend commands from `claims-ui`:

```powershell
npm run build -- --configuration development
npm run build -- --configuration production
npm test -- --watch=false
```

The package scripts define `npm run build` and `npm test`; the production configuration is the Angular build default.

Backend commands from the repository root:

```powershell
dotnet restore ClaimsManagement.slnx
dotnet build ClaimsManagement.slnx
```

No .NET test project is present in the solution. Angular has a test script and an `app.spec.ts`; see the verification notes below for the current runner limitation.

## Testing and verification

Verification is separated by evidence type:

- **Developer-reported manual workflow checks:** Assignment, Request Information, Provide Information, and Settle were reported as manually exercised with successful responses and expected resulting statuses. Approve/Reject have been implemented, but are not claimed as manually tested.
- **Read-only API checks:** Tracking an existing claim returned the claim; an invalid claim number returned HTTP 404. Email filtering returned matching claims for a matching email and an empty list for a nonmatching email. These checks did not create or update claims.
- **Dashboard comparison:** Dashboard totals, status counts, officer workloads, and outstanding liability were compared with the claims API. One historical snapshot contained six claims and RM 3,200.00 in outstanding liability; these are snapshot values and not fixed project data.
- **Validation pipeline check:** An invalid settlement request returned HTTP 400 `ValidationProblemDetails` before claim lookup. The request did not alter claim records.
- **Claim submission/persistence:** Code inspection confirms that the create handler adds the claim and audit event and calls `SaveChangesAsync`. This documentation task did not submit a claim or independently verify a new SQL Server row.
- **Builds run while preparing this documentation:** Angular development build succeeded; .NET solution build succeeded. The Angular production build failed the component CSS size budget (details below).
- **Angular automated tests:** `npm test -- --watch=false` was attempted, but test execution stopped before the test cases ran because the installed environment could not load `@rollup/rollup-win32-x64-msvc`. The existence of `app.spec.ts` is not evidence that tests passed.

No claim records were created or modified during documentation preparation.

## Known limitations and security considerations

- **No real authentication or authorization:** There is no configured authentication scheme, authorization middleware, or role enforcement. Staff actions and claim/audit endpoints are not protected by authenticated roles.
- **Demo `performedBy`:** Values such as `Demo Claims Officer` are supplied by the client and are not authenticated identities. Audit attribution is therefore not authoritative.
- **Email is not ownership proof:** `claimantEmail` filtering is a demo lookup only. A user-provided email does not prove ownership, and the unfiltered claims list is also exposed by the API.
- **Sensitive claim data exposure:** Claim responses include claimant and incident information, additional information, decision reasons, and assigned officer details. The tracking UI displays some of those fields. The audit endpoint returns audit details and performer labels and is not authorization-protected.
- **Limited automated tests:** No .NET test project is present. The current Angular test command could not load a Rollup platform-specific optional dependency, so its test cases did not run in the verified environment.
- **Validation scope:** FluentValidation currently covers claim creation and the staff commands. List-query filters and claimant identity are not authorization concerns enforced by those validators.
- **Create amount rule differs by layer:** The frontend requires an amount greater than zero, but the API validator allows zero. Align these rules if zero-valued claims should be consistently rejected.
- **Database setup:** A migration exists, but schema application is a manual setup step. There is no seed data or startup migration.
- **Production Angular build:** The current production build exceeds the `app.css` component style error budget. No CSS changes were made as part of documentation.
- **Assessment criteria:** The full take-home assessment brief/rubric is not present in this repository, so complete rubric coverage cannot be verified.

## Build warnings observed

Current local verification results:

| Command | Result |
|---|---|
| `npm run build -- --configuration development` | Succeeded; warns that `RouterOutlet` is imported but unused. |
| `npm run build -- --configuration production` | Failed: `app.css` is 11.64 kB, exceeding the configured 8.00 kB error budget (4.00 kB warning budget). |
| `dotnet build ClaimsManagement.slnx` | Succeeded with warnings. |
| `npm test -- --watch=false` | Test cases did not run; Rollup’s `@rollup/rollup-win32-x64-msvc` module was unavailable in the installed environment. |

The .NET build reported NuGet audit warnings for `System.Security.Cryptography.Pkcs` 9.0.11 (moderate and high severity advisories) and a duplicate `using` warning in `ClaimsController.cs`. Review current package advisories before any dependency changes; this README does not claim that the dependency issue has been remediated.

## Future improvements

1. Add authentication and role-based authorization for claimant and staff endpoints.
2. Verify claimant ownership server-side rather than trusting submitted email or claim number.
3. Protect the audit endpoint and restrict sensitive claim fields to appropriate roles.
4. Add focused .NET tests and expand Angular tests for validation, status transitions, and API error cases.
5. Resolve the Angular production CSS budget and repair the test runner’s optional Rollup dependency installation.
6. Review and safely update vulnerable dependencies.
7. Document reliable database provisioning and migration steps for a clean development environment.

## AI assistance disclosure

This project was developed with AI assistance. See [AI_WORK_JOURNAL.md](./AI_WORK_JOURNAL.md) for a chronological account of how AI tools and developer verification contributed.
