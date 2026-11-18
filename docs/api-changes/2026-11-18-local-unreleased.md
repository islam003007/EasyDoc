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
| PATCH | `/admin/doctors/{id}` | admin | Partial doctor update. Visibility-only requests now validate correctly. |

## Response additions

- Appointment responses add `patientName` and `doctorName` (nullable).
- Admin doctor list is ordered and includes non-visible doctors.
- Appointment statuses remain the existing numeric/string enum representation.

## Admin visibility fix

- `PATCH /admin/doctors/{id}` previously failed with a validation error for a visibility-only
  request because optional field rules were conditioned on the command object rather than the
  property (`When(x => x != null)`). All optional admin doctor rules now check the property, and
  the nullable appointment-duration range rule runs only when a duration is supplied.
- The same optional-field condition bug was fixed for doctor registration description/profile
  picture fields.
- Added validator regression tests. The admin UI toggle now writes the new visibility and the
  patient directory reflects it.

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

- 40 unit tests pass locally (with `DOTNET_ROLL_FORWARD=Major` when only .NET 10 is installed).
- Admin doctor show/hide was verified against the live local API, including restoring the record.
- No database-backed integration suite yet.

## Frontend

`EasyDoc.Web/` is a local addition, not an API change: role-based patient/doctor/admin UI,
search/booking/appointments, schedules and admin visibility, with the Friendly Care pink/blue logo
theme, standard patient password login and patient self-registration. Demo mode auto-confirms new
patient accounts so the client can immediately sign in. Doctor/admin registration remains absent.
The polish pass raises functional type to at least 12px, removes raw account/record IDs and
cookie/session implementation copy from the UI, and labels the admin as an intentional sandbox
review seat with a clear demo-environment notice.
