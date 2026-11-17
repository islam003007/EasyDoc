# EasyDoc Demo Frontend Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development or superpowers:executing-plans. Steps use checkbox syntax.

**Goal:** Add the demo/role endpoints EasyDoc needs and build a role-based React client for patient, doctor and admin flows.

**Architecture:** Keep existing CQRS/minimal API patterns. Add thin demo-auth, lookup, profile and admin-list endpoints. The SPA uses cookie auth through a same-origin proxy and TanStack Query.

**Tech Stack:** ASP.NET Core 9, custom CQRS, EF Core, SQL Server; React 19, TypeScript, Vite, TanStack Query, date-fns.

**Spec:** `docs/superpowers/specs/2026-11-18-easydoc-demo-frontend-design.md`

## Global Constraints
- No password in frontend; demo login uses a server-side allowlist and `Demo:Enabled`.
- No email/registration flow in the demo.
- Do not change existing endpoint contracts; additive fields are fine.
- Keep role policies and ownership checks.
- Use existing seed data; no file uploads.
- Same-origin proxy; no permissive CORS.

## Review Focus
- Demo login cannot select an arbitrary user.
- Doctor/patient ownership checks still apply to appointment actions.
- Availability date/time handling respects `DateOnly`/`TimeOnly` JSON values.
- Admin list includes invisible doctors and visibility changes persist.
- Role discovery is correct immediately after login.

---

### Task 1: Demo auth and identity
**Files:** `EasyDoc.Api/Endpoints/Auth/Me.cs`, `DemoUsers.cs`, `DemoLogin.cs`, `EasyDoc.Api/DependencyInjection.cs`, appsettings.
**Interfaces:** `GET /auth/me`, `GET /auth/demo-users`, `POST /auth/demo-login { key }`; `Demo:Enabled`.
- [ ] Add demo config and seeded-account allowlist.
- [ ] Use claims/profile IDs; return role and display name.
- [ ] Demo sign-in via `SignInManager.SignInAsync`; gate disabled.
- [ ] Build/test; commit.

### Task 2: Reference lookups
**Files:** `EasyDoc.Api/Endpoints/Lookups/*`, `EasyDoc.Core/.../Lookups`, `Feature` enum.
**Interfaces:** cities, departments, governorates lists.
- [ ] Add read-only queries/handlers and endpoints.
- [ ] Build/test; commit.

### Task 3: Missing role endpoints
**Files:** `Doctors/GetMe.cs`, `Doctors/Admin/Get.cs`, `Appointments/CancelByPatient.cs`, service/query additions.
**Interfaces:** `GET /doctors/me`, `GET /admin/doctors`, `POST /appointments/{id}/cancel/patient`.
- [ ] Add doctor-me endpoint using existing query.
- [ ] Add paged admin doctor list including invisible.
- [ ] Add patient-owned cancellation with role/ownership checks.
- [ ] Build/test; commit.

### Task 4: SPA scaffold and auth
**Files:** `EasyDoc.Web/` Vite app, proxy, API client, role guard, login/demo picker, design tokens.
**Interfaces:** Consumes `/auth/*`; produces `useSession`.
- [ ] Scaffold/install.
- [ ] Demo role cards; standard login link only; logout/session.
- [ ] Visual system.
- [ ] Build; commit.

### Task 5: Patient flows
**Files:** pages/components for search, doctor detail, booking, appointments.
- [ ] Search with department/city filters; doctor list.
- [ ] Doctor profile + date picker + available slots + book.
- [ ] Appointments list + patient cancel.
- [ ] Build; commit.

### Task 6: Doctor and admin flows
**Files:** doctor dashboard, schedule, appointments actions, admin tables.
- [ ] Doctor appointment actions and schedule/overrides.
- [ ] Admin doctor visibility and patient list.
- [ ] Build/browser-check; commit.

### Task 7: Verification and handoff
- [ ] `dotnet build EasyDoc.sln`, unit tests (roll-forward if needed).
- [ ] `npm run build`, TypeScript clean.
- [ ] Demo credentials/README updated.
- [ ] Commit.
