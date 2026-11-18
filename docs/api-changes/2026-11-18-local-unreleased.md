# EasyDoc — local API changes (not yet in `origin/main`)

Baseline: `5731d10` (`origin/main`). These changes are local-only and should be reviewed before
push. The demo frontend is in `EasyDoc.Web/`.

## API surface

| Method | Route | Auth | Change |
|---|---|---|---|
| GET | `/auth/me` | cookie | `{ userId, email, role, displayName, patientId?, doctorId? }`. |
| GET | `/auth/demo-users` | anonymous | Seeded patient/doctor/admin roster; demo mode only. |
| POST | `/auth/demo-login` | anonymous | `{ key }`; signs in an allow-listed seeded profile; no password; demo mode only. |
| GET | `/lookups/departments` | anonymous | Department `{ id, name }` list. |
| GET | `/lookups/cities` | anonymous | City `{ id, name, governorateId }` list. |
| GET | `/lookups/governorates` | anonymous | Governorate `{ id, name }` list. |
| GET | `/doctors/me` | doctor | Existing `MeDoctorResponse` for the signed-in doctor. |
| GET | `/admin/doctors?pageNumber=&pageSize=` | admin | Doctor list including invisible profiles. |
| GET | `/appointments` | patient | Existing appointment creation route remains unchanged. |
| POST | `/appointments/{appointmentId}/cancel/patient` | patient | Patient-owned cancellation; only pending/scheduled; 404/403/409 problem paths. |

## Response additions

- Appointment responses add `patientName` and `doctorName` (nullable).
- Admin doctor list is ordered and includes non-visible doctors.
- Appointment statuses remain the existing numeric/string enum representation.

## Configuration

- `Demo:Enabled` default `false`; `true` in `appsettings.Development.json`.
- `Demo:AutoConfirmRegistrations` is implemented in `UserService.CreateUserAsync`; when true, new
  identities are confirmed immediately and the log-only confirmation email is skipped. It is `true`
  in `appsettings.Development.json` and false by default. Keep it false in production.
- No new database migrations.

## Data/seed changes

- Demo city labels are now Cairo, Giza, Alexandria, Mansoura, Aswan and Hurghada.
- The Sohag governorate label was removed from source seed/runtime demo data.
- Clinic addresses in the seeded data match the renamed cities.
- The admin doctor list ordering bug was fixed so EF translates it correctly.

## Tests

- 38 unit tests pass locally (with `DOTNET_ROLL_FORWARD=Major` when only .NET 10 is installed).
- No database-backed integration suite yet.

## Frontend

`EasyDoc.Web/` is a local addition, not an API change: role-based patient/doctor/admin UI,
search/booking/appointments, schedules and admin visibility, with the Friendly Care pink/blue logo
theme, standard patient password login and patient self-registration. Demo mode auto-confirms new
patient accounts so the client can immediately sign in. Doctor/admin registration remains absent.
