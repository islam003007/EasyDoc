# EasyDoc

**EasyDoc** is a learning and portfolio project inspired by Milan Jovanović and Steve Smith (Ardalis), designed to showcase clean architecture, domain-driven design principles, and sophisticated API design in ASP.NET Core.  

It is a **doctor scheduling application** where patients can search for doctors and book appointments based on availability. Think of it like an e-commerce platform for healthcare scheduling.

> ⚠️ **Work in Progress:** The project is not yet fully complete, and the API has not been fully tested. Use for learning or demonstration purposes only.  

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

> ⚠️ The project is still a work in progress. The API has not been fully tested, so running it locally is **not yet recommended**.  

When ready for local development, the project uses **User Secrets** to manage the SQL Server connection string:

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

## Future Work / Roadmap

- Complete all API endpoints and services  
- Add full test coverage  
- Expand search algorithm for advanced filtering  
- Optionally integrate Docker for local development  
- Polish API documentation with Swagger  

---

### Notes

- Designed for **interviews and portfolio demonstration**   
- Read and write separation is key to enforce clean architecture principles  
- Generic repository **enforces writes only via aggregate roots** without overengineering.