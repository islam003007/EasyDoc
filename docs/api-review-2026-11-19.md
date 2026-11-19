# EasyDoc API review — 2026-11-19

Scope: correctness and safety of the appointment, availability, history, schedule and
authorization rules. The API's public surface was not redesigned.

## Fixed in this pass

| Severity | Finding | Fix | Verification |
| --- | --- | --- | --- |
| High | Canceled appointments blocked same-day rebooking | Duplicate specification now excludes `Canceled` | 6 specification tests; live rebook after cancel succeeded |
| High | Same-day duplicate rule was missing on reschedule | Added the duplicate specification with the moved appointment excluded | Specification tests cover the exclusion; live flow checked |
| High | Clinic-local times were compared with UTC | Added `ClinicClock` (Africa/Cairo with an Egypt fallback) and used it for create, reschedule, accept and slot generation | Clock unit tests |
| High | A doctor could accept a pending request in the past | Accept now rejects non-future appointments | Clock/service rule |
| High | Availability returned past times for today | Slot generation filters against clinic-local now | Slot calculation tests |
| Medium | History required a currently `Scheduled` visit; completed-only doctors could not read history | History access now requires any non-canceled appointment with the patient | Code review; validator tests |
| Medium | History, patient and doctor appointment queries allowed page number 0 | Validators now require page number ≥ 1 | Validator tests |
| Low | Duplicate/error messages were developer-facing and grammatically inconsistent | Human-readable messages for past, duplicate, reschedule and history errors | Frontend error-copy tests |
| Low | An available override with missing times could throw in slot generation | Slot endpoint returns no slots for malformed overrides instead of throwing | Defensive guard |
| Low | Client comments claimed schedule day/date updates were ignored | Removed; the API does update both | Code review |

Past-date creation was already rejected by command validation (HTTP 400). A second
service-level `IsInTheFuture` guard was added for defense in depth.

## Frontend prevention added

- Same-day active appointment with the same doctor is blocked before requesting, with a link to
  My visits and conflicted dates marked on the rail.
- The same check is applied when rescheduling, excluding the appointment being moved.
- Known API conflicts are translated into actionable copy and the slot list refreshes.
- The schedule UI no longer offers `Add hours` for a day that already has a window, and omits taken
  days from the add/edit selects.
- Override creation/edit rejects an existing date before submission.
- Registration validation now matches the ASP.NET Identity password policy (8+ characters, upper,
  lower, digit, symbol).

## Still open / needs a product decision

- Hidden doctors can still be fetched by direct ID and their slots can be requested. The current
  data model uses visibility for search/directory listing; whether direct booking must be blocked
  is a product decision.
- The API has unit tests but no SQL Server-backed endpoint integration suite. Live probes and the
  frontend E2E suite cover the critical endpoint paths.
- Enum member `completed` is lower-case and `/Complete` is capitalized. Contract-compatible but
  cosmetically inconsistent.
- Rate limiting and CSRF policy should be reviewed before a real deployment; the current demo uses
  same-site cookies and demo-mode gating, but that is not a production security review.

## Test status

- `EasyDoc.UnitTests`: **81 passed**.
- Live probes after restart:
  - past creation: `400`
  - active same-day duplicate: `409` with the corrected message
  - rebooking a canceled same-day visit: `200`, then canceled cleanly
- Frontend unit tests: **39 passed** at the time of this note; the suite is still growing.
