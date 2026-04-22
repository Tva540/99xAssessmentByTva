# 99x Assessment By Thiva

Accounts balance viewer for Jondell Corp. Monthly balance uploads (`.xlsx`, `.xls`, `.tsv`, `.txt`); role-based viewing per the assessment brief.

**Stack:**

- ASP.NET Core 10
- Angular 21
- EF Core
- PostgreSQL
- JWT
- Azure App Service + Key Vault

---

## Prerequisites

- **.NET SDK** 10.0.100 +
- **Node.js** 20 LTS +
- **npm** 10 + (bundled with Node.js; required for Angular + Playwright)
- **PostgreSQL** 14 + on `localhost:5432`

First time on a machine:

```bash
dotnet dev-certs https --trust
```

Update the PostgreSQL password in `src/backend/Server/appsettings.json`. The schema is created automatically on first run via EF Core migrations.

---

## Run

```bash
dotnet run --project src/backend/Server
```

- **ASP.NET Core SPA Proxy** auto-launches `npm start` and redirects the browser to `https://localhost:50810`.
- **MSBuild** auto-installs `node_modules/` on first build if missing.

### Default credentials

| Email | Password | Role | Lands on |
|---|---|---|---|
| `admin@jondell.local` | `Admin@12345` | Admin | `/upload`, `/reports` |
| `viewer@jondell.local` | `Viewer@12345` | Viewer | `/balances` |

Admins and viewers are strictly segregated per the brief — admins cannot view the balances page; viewers cannot upload or view reports. Self-registration at `/register` creates a Viewer account.

### API documentation (Development only)

- OpenAPI spec: `https://localhost:7039/openapi/v1.json`
- Scalar UI: `https://localhost:7039/scalar/v1`

---

## Project structure

```
x99AssessmentByTva/
├── src/
│   ├── backend/
│   │   ├── Domain/          Entities, enums, constants
│   │   ├── Application/     MediatR handlers, validators, abstractions
│   │   ├── Infrastructure/  EF Core, Identity, JWT, file parsers
│   │   └── Server/          ASP.NET Core host, controllers, Program.cs
│   └── frontend/            Angular 21 SPA (features/, core/, shared/, e2e/)
├── tests/
│   └── Application.UnitTests/   xUnit
├── Directory.Build.props        Solution-wide MSBuild properties
├── Directory.Packages.props     Central NuGet version management
├── global.json                  .NET SDK version pin
└── x99AssessmentByTva.slnx      Solution file
```

**Architecture:** Clean Architecture with CQRS on MediatR. Dependencies point inward:

```
┌────────┐     ┌─────────────┐     ┌────────────────┐     ┌────────┐
│ Domain │ ◄── │ Application │ ◄── │ Infrastructure │ ◄── │ Server │
└────────┘     └─────────────┘     └────────────────┘     └────────┘
```

Commands and Queries live in `Application/*/Commands` and `Application/*/Queries`. `ValidationBehaviour` and `UnhandledExceptionBehaviour` wrap every request.

**Configuration** — Development reads `appsettings.json`; Production overrides secrets from **Azure Key Vault** when `VaultUri` is set.

---

## Tests

One command runs every suite:

```bash
dotnet build x99AssessmentByTva.slnx
```

| Command | Runs tests? |
|---|---|
| `dotnet build x99AssessmentByTva.slnx` | ✅ |
| `dotnet test` | ✅ |
| `dotnet run --project src/backend/Server` | ❌ (builds Server only) |

The test projects are not referenced by `Server.csproj` (production must not depend on test code). **Always verify with the solution-level build before submitting.**

Skip tests during a build: `dotnet build -p:SkipTests=true`.

### End-to-end (Playwright)

Start the app in one terminal, then in another:

```bash
cd src/frontend
npm run e2e:install   # one-time: installs Chromium
npm run e2e
```

Specs in `src/frontend/e2e/`: `login`, `register`, `upload`, `balances`.

---

## Azure PaaS services

| Service | Resource | Purpose |
|---|---|---|
| App Service | `x99AssessmentByTva` | Hosts backend + compiled SPA |
| App Service Plan | `ASP-x99AssessmentByTvagroup-90af` | Compute tier |
| Database for PostgreSQL | `tav-server` | Application database |
| Key Vault | `AssessmentByTva`, `keyvaultassessment` | Production secrets (JWT key, connection string) |
| API Management | `AssessmentTest` | API gateway |
| VNet + Private DNS Zone | `vnet-bpjpkelu`, `privatelink.postgres.database.azure.com` | Private Link to PostgreSQL |

Resource group: `x99AssessmentByTva_group` · Region: Malaysia West.

---