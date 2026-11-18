# EasyDoc

**EasyDoc** is a learning and portfolio project inspired by Milan Jovanović and Steve Smith (Ardalis), designed to showcase clean architecture, domain-driven design principles, and sophisticated API design in ASP.NET Core.  

It is a **doctor scheduling application** where patients can search for doctors and book appointments based on availability. Think of it like an e-commerce platform for healthcare scheduling.

> **Portfolio demo:** The API and Friendly Care client use seeded clinic records. Demo seats open
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
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YourConnectionStringHere"
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

## Demo frontend

`EasyDoc.Web/` is the role-based portfolio client: React 19 + TypeScript + Vite, TanStack Query,
and custom CSS. Its Friendly Care theme uses the pink/blue EasyDoc mark, Plus Jakarta Sans and IBM
Plex Sans Arabic. It has patient, doctor and administrator flows, one-click demo seats, standard
patient password sign-in and patient self-registration.

```bash
cd EasyDoc.Web
npm install
npm run dev
# http://localhost:5174
```

The Vite proxy sends `/api` to `http://localhost:5215` with the prefix stripped. Use
`VITE_API_TARGET` to override the API target. See `EasyDoc.Web/README.md` for flows and build notes.

Demo login is enabled only with `Demo:Enabled=true` (true in
`appsettings.Development.json`). The seeded demo profiles are patient `sara@example.com`,
doctor `ahmed@example.com`, and `admin@example.com`.
