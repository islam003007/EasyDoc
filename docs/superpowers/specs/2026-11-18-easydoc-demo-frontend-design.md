# EasyDoc Demo Frontend — Design

## Intent
A low-friction, role-based demo of EasyDoc's real scheduling API for a portfolio. It
should show patient discovery and booking, doctor operations, and admin management
without public signup, email confirmation or upload friction.

## Goals
- One-click demo sessions for a patient, a doctor and an admin. No passwords in UI.
- Patient: search/filter doctors, view a doctor, choose an available slot, book,
  review appointments.
- Doctor: see today's/upcoming appointments, accept/cancel/complete, inspect schedule
  and overrides, view own profile.
- Admin: browse doctors and patients, toggle doctor visibility.
- Show the bilingual search and the serializable booking outcome where relevant.
- Use seeded data and real API calls.

## Non-goals
Public registration, email confirmation/delivery, password reset, doctor account
creation, ID-card/profile uploads, payments, notifications.

## Demo data
Seeded confirmed accounts:
- Patient: `sara@example.com` (سارة محمد)
- Doctor: `ahmed@example.com` (أحمد حسن)
- Admin: `admin@example.com`
Additional seeded doctors/patients can be selectable for a richer search demo.

## API changes
- `GET /auth/me` → `{ userId, email, role, displayName, patientId?, doctorId? }`.
- `GET /auth/demo-users` → role/label/email/key list, demo mode only.
- `POST /auth/demo-login` `{ key }` → cookie sign-in for a seeded account, demo mode only.
- `GET /lookups/departments`, `/lookups/cities`, `/lookups/governorates`.
- `GET /doctors/me` → existing `MeDoctorResponse`.
- `GET /admin/doctors` → paged list including invisible doctors.
- `POST /appointments/{id}/cancel/patient` → patient-owned pending/scheduled cancellation.
- Demo gated by `Demo:Enabled`; no CORS dependency (same-origin proxy).

## UI
Routes:
- `/login` — role picker with patient/doctor/admin demo cards.
- `/patient` — search landing, filters, doctor results.
- `/patient/doctors/:id` — profile, clinic, description, date + available slots, book.
- `/patient/appointments` — list, statuses, cancel.
- `/doctor` — dashboard/upcoming appointments.
- `/doctor/schedule` — weekly schedules and overrides.
- `/doctor/profile` — own profile.
- `/admin` — doctors/patients tables, visibility toggles.
- `/account` — session info/logout; maybe role-specific profile.

Design:
- Clinical editorial: warm white, deep forest green, cobalt links, terracotta status
  accents. Fraunces display, Instrument Sans body, IBM Plex Mono metadata.
- Search is the hero; availability as a clear time-slot grid; doctor cards use monograms
  because seeded image URLs are placeholders.
- Responsive: split layouts collapse to single column; tables become cards.
- Real loading/empty/error states.

## Frontend stack
Vite + React + TypeScript, React Router, TanStack Query, date-fns.
Cookie credentials; Vite proxy `/api` → `http://localhost:5215` with `/api` stripped.

## Acceptance
- One click enters Sara, Ahmed or Admin.
- Patient can search, filter, open a doctor, load slots for a date, book and see it.
- Doctor can list/accept/cancel/complete appointments and view schedules.
- Admin can list doctors/patients and toggle visibility.
- API and frontend build; routes are guarded by role.
