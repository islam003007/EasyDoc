# EasyDoc

**EasyDoc** is a learning and portfolio project inspired by Milan Jovanović and Steve Smith (Ardalis), designed to showcase clean architecture, domain-driven design principles, and sophisticated API design in ASP.NET Core.  

It is a **doctor scheduling application** where patients can search for doctors and book appointments based on availability. Think of it like an e-commerce platform for healthcare scheduling.

> **Portfolio demo:** The API and its frontend clients use seeded clinic records. Demo seats open
> in one click; patient email/password registration is available with demo auto-confirm. Real email
> delivery, uploads, payments, doctor self-registration and production hardening are out of scope.

---

## Features

- Minimal API architecture using **ASP.NET Core 9** (upgradeable to .NET 10)  
- Domain-driven design with a **loosened DDD approach**:  
  - Domain protects its invariants  
  - Services handle most business logic while respecting domain rules  
- **CQRS** with a custom pipeline/middleware (similar to MediatR, but fully custom-built)  
- Read and write sides are separate:  
  - **Write Side:** API handler → Command handler → Service → **Generic Repository (enforces writes via aggregate roots only)** → Domain → Service saves changes  
  - **Read Side:** API handler → Query handler → Readonly DbContext  
- **Sophisticated search algorithm**:  
  - Database-only, no external search engine  
  - Normalizes Arabic and English names  
  - Uses **Double Metaphone** for English names  
  - Generates a search index with normalized names and phonetic keys  
  - Search is executed via a stored procedure in SQL Server, performing strict and fuzzy searches with ranking  

---

## Architecture Overview

```
Write Side:        Read Side:
API Handler        API Handler
   |                  |
Command Handler     Query Handler
   |                  |
Service           Readonly DbContext
   |
Generic Repository
   |
Domain
   |
Service calls SaveChanges()
```

- **Write Side** uses a **generic repository**, not a full classic repository pattern. Its main purpose is to **enforce that all database changes go through aggregate roots**, keeping the design simple while protecting invariants.  
- **Read Side** focuses on fast queries without enforcing invariants, allowing complex joins directly via DbContext.  

---

## Libraries & Tools Used

- **EF Core** – ORM  
- **ASP.NET Core Minimal API** – lightweight API design  
- **ASP.NET Core Identity** – user management  
- **Ardalis.GuardClauses** – argument validation  
- **Ardalis.Specification** – repository pattern with specification support  
- **Scrutor** – automatic handler detection and DI  
- **SQL Server** – database (search algorithm uses SQL Server features, but can be adapted to other DBs with some migration adjustments)  

---

## Inspiration & Learning

This project was heavily inspired by:  

- [Milan Jovanović](https://www.youtube.com/@MilanJovanovicTech), whose guidance on clean architecture helped shape the design decisions in EasyDoc  
- [Steve Smith (Ardalis)](https://ardalis.com/) and his **eShopOnWeb** sample, which helped shape the use of a generic repository pattern and clean architecture decisions  

EasyDoc is primarily a **learning and portfolio project**, showcasing:  
- Clean architecture implementation  
- Loosened DDD with invariant protection  
- Custom CQRS pipeline  
- Advanced search implementation  

---

## Getting Started

> The demo client uses seeded, confirmed accounts and a one-click demo login. You do not need to
> use the email confirmation flow to try it.

For local development, the project uses **User Secrets** to manage the SQL Server connection string:

```bash
dotnet user-secrets init --project EasyDoc.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YourConnectionStringHere" --project EasyDoc.Api
```

The seeded demo accounts also need public demo credentials. Set them with user
secrets, or provide the equivalent environment variables when running in a
container:

```bash
dotnet user-secrets set "DataSeed:UsersPassword" "REPLACE_WITH_VALID_DEMO_PASSWORD_123!" --project EasyDoc.Api
dotnet user-secrets set "DataSeed:AdminPassword" "REPLACE_WITH_VALID_DEMO_PASSWORD_123!" --project EasyDoc.Api
dotnet user-secrets set "DataSeed:AdminEmail" "admin@example.com" --project EasyDoc.Api
```

Replace the password placeholder before running a fresh seed. It must have at
least 8 characters, uppercase, lowercase, and a digit. The API fails during
startup if the password is missing or invalid. Changing the value does not
update users that were already seeded; reset the demo database or use the
demo-reset service.

`.env.example` documents the environment-variable names. It is a reference
file, not automatically loaded by `dotnet run`.

Start the API:

```bash
dotnet run --project EasyDoc.Api
```

The API applies EF migrations at startup, then runs the seeders. The API listens
on `http://localhost:5215`; Swagger is at `/swagger` in Development.

### Docker

Build from the repository root:

```bash
docker build -f EasyDoc.Api/Dockerfile -t easydoc-api .
```

The image listens on port `8080`. The API applies EF migrations at startup.
Supply the connection string and demo credentials through the deployment
environment.

When the API is behind a TLS-terminating proxy, set
`ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`. CORS origins are environment-only:

```env
Frontend__AllowedOrigins__0=https://<frontend-origin>
```

## Email Sending

The project uses a **log-only email sender** in development. It writes the email subject, recipient,
and body (including confirmation and password-reset links) to the application logs. No external email
service, API token, or package feed is required.

For production, replace `LogEmailSender` with a real provider. The FalkElTawfik project has a
Resend-based sender that can be adapted, or you can point an SMTP sender at a local Mailpit container
for a visual sandbox inbox.

---

## Testing

The unit test project covers the search normalization primitives (Arabic normalization,
input sanitization, and Double Metaphone keys) without requiring SQL Server:

```bash
dotnet test EasyDoc.UnitTests/EasyDoc.UnitTests.csproj
```

CI restores from nuget.org and runs the unit tests; no GitHub Packages token is required.

---

## License

Some files are licensed under the **Apache 2.0 License**. Refer to the respective files for license details.  

---

## Demo scope

The demo uses seeded clinic records and demo seats. Patient email/password self-registration is
available and auto-confirms in Development so the flow works without a real inbox. Doctor/admin
accounts remain seeded. Real email delivery, uploads, payments and production hardening remain
outside this build.

---

### Notes

- Designed for **interviews and portfolio demonstration**   
- Read and write separation is key to enforce clean architecture principles  
- Generic repository **enforces writes only via aggregate roots** without overengineering.

## Frontend

The React clients live in a separate repository:
[EasyDoc.Frontend](https://github.com/islam003007/EasyDoc.Frontend).

- `EasyDoc.Web/` — Friendly Care, `http://localhost:5174`
- `EasyDoc.Web2/` — Open Hours, `http://localhost:5175`

Demo login is enabled only with `Demo:Enabled=true` (true in
`appsettings.Development.json`). The seeded demo profiles are patient `sara@example.com`,
doctor `ahmed@example.com`, and the sandbox administrator `admin@example.com`.

### Demo security model

This is a deliberately open portfolio sandbox, not the production authorization model.

- Demo authentication is a separate endpoint from the normal login and is disabled by default.
  Set `Demo:Enabled=true` only in a dedicated demo environment with a separate demo database.
- The demo admin is a seeded sandbox seat. It can inspect directories and toggle doctor visibility.
  The destructive admin delete endpoints return `404` while `Demo:Enabled=true`.
- Demo seed passwords come from `DataSeed:UsersPassword` and `DataSeed:AdminPassword`
  (environment variables or user secrets). They are intentionally public demo
  credentials and must never be reused outside a disposable demo database.
- Patient self-registration writes only to the demo database. Do not enter real patient information.
- When `Demo:Reset:Enabled=true`, a background service wipes the demo rows and
  re-seeds them every `Demo:Reset:IntervalHours` hours. This is only safe on a
  dedicated demo database.
- `RateLimit` applies a per-IP fixed window (default: 120 requests per minute).
- Production must keep `Demo:Enabled=false`, `Demo:Reset:Enabled=false`, and use the regular
  cookie/role policies.
