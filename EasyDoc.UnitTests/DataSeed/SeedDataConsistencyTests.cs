using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Infrastructure.Data.DataSeed;
using EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;
using Xunit;

namespace EasyDoc.UnitTests.DataSeed;

public class SeedDataConsistencyTests
{
    [Fact]
    public async Task DevelopmentSeed_HasEnoughDoctorsPatientsAndAppointmentStates()
    {
        var doctors = await LoadDoctors();
        var patients = await LoadPatients();
        var appointments = await LoadAppointments();
        var doctorUsers = await JsonDataLoader.LoadJsonAsync<UserSeedMaterializer>("doctorUsers.json");
        var patientUsers = await JsonDataLoader.LoadJsonAsync<UserSeedMaterializer>("patientUsers.json");

        Assert.True(doctors.Count >= 6, $"Expected at least 6 doctors, found {doctors.Count}.");
        Assert.True(patients.Count >= 5, $"Expected at least 5 patients, found {patients.Count}.");
        Assert.Contains(doctors, doctor => doctor.IsVisible);
        Assert.Contains(doctors, doctor => !doctor.IsVisible);

        var durations = doctors.Select(doctor => doctor.DefaultAppointmentTimeInMinutes).ToHashSet();
        Assert.Contains(20L, durations);
        Assert.Contains(30L, durations);
        Assert.Contains(45L, durations);

        Assert.Equal(appointments.Count, appointments.Select(appointment => appointment.Id).Distinct().Count());
        Assert.Contains(appointments, appointment => appointment.Status == AppointmentStatus.Pending);
        Assert.Contains(appointments, appointment => appointment.Status == AppointmentStatus.Scheduled);
        Assert.Contains(appointments, appointment => appointment.Status == AppointmentStatus.completed);
        Assert.Contains(appointments, appointment => appointment.Status == AppointmentStatus.Canceled);

        Assert.Equal(doctors.Count, doctors.Select(doctor => doctor.Id).Distinct().Count());
        Assert.Equal(patients.Count, patients.Select(patient => patient.Id).Distinct().Count());
        Assert.All(doctors, doctor => Assert.Contains(doctorUsers, user => user.Id == doctor.UserId));
        Assert.All(patients, patient => Assert.Contains(patientUsers, user => user.Id == patient.UserId));
        Assert.All(patients, patient => Assert.StartsWith("+20", patient.PhoneNumber));

        var upcoming = appointments.Count(appointment =>
            appointment.Date >= new DateOnly(2026, 11, 19) &&
            appointment.Status is AppointmentStatus.Pending or AppointmentStatus.Scheduled);

        Assert.True(upcoming >= 10, $"Expected at least 10 upcoming appointments, found {upcoming}.");

        Assert.Contains(appointments, appointment =>
            appointment.Examination is not null &&
            appointment.Examination.Diagnosis.Contains("hypertension", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(appointments, appointment =>
            appointment.Examination is not null &&
            appointment.Examination.Diagnosis.Contains("fever", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(appointments, appointment =>
            appointment.Examination is not null &&
            appointment.Examination.Diagnosis.Contains("strain", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(appointments, appointment =>
            appointment.Examination is not null &&
            appointment.Examination.Diagnosis.Contains("dental", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DevelopmentSeed_AppointmentsFitTheirDoctorsAvailability()
    {
        var doctors = await LoadDoctors();
        var appointments = await LoadAppointments();
        var doctorsById = doctors.ToDictionary(doctor => doctor.Id);

        foreach (var appointment in appointments)
        {
            Assert.True(doctorsById.ContainsKey(appointment.DoctorId),
                $"Appointment {appointment.Id} references an unknown doctor.");

            var doctor = doctorsById[appointment.DoctorId];
            var duration = (appointment.EndTime.ToTimeSpan() - appointment.StartTime.ToTimeSpan()).TotalMinutes;

            Assert.Equal(doctor.DefaultAppointmentTimeInMinutes, (long)duration);

            var dateOverride = doctor.ScheduleOverrides.SingleOrDefault(overrideEntry =>
                overrideEntry.Date == appointment.Date);

            if (dateOverride is not null)
            {
                Assert.True(dateOverride.IsAvailable,
                    $"Appointment {appointment.Id} falls on a doctor day off ({appointment.Date}).");
                Assert.NotNull(dateOverride.StartTime);
                Assert.NotNull(dateOverride.EndTime);
                Assert.True(appointment.StartTime >= dateOverride.StartTime.Value &&
                            appointment.EndTime <= dateOverride.EndTime.Value,
                    $"Appointment {appointment.Id} falls outside the {appointment.Date} override.");
                continue;
            }

            var schedule = doctor.Schedules.SingleOrDefault(entry =>
                entry.DayOfWeek == appointment.Date.DayOfWeek);

            Assert.NotNull(schedule);
            Assert.True(appointment.StartTime >= schedule.StartTime &&
                        appointment.EndTime <= schedule.EndTime,
                $"Appointment {appointment.Id} falls outside the weekly schedule.");
        }
    }

    [Fact]
    public async Task DevelopmentSeed_ActiveAppointmentsDoNotOverlap()
    {
        var appointments = await LoadAppointments();

        var activeAppointments = appointments.Where(appointment =>
            appointment.Status is AppointmentStatus.Pending or AppointmentStatus.Scheduled);

        foreach (var group in activeAppointments.GroupBy(appointment => new
                 {
                     appointment.DoctorId,
                     appointment.Date
                 }))
        {
            var ordered = group.OrderBy(appointment => appointment.StartTime).ToList();

            for (var index = 1; index < ordered.Count; index++)
            {
                Assert.True(ordered[index - 1].EndTime <= ordered[index].StartTime,
                    $"Appointments {ordered[index - 1].Id} and {ordered[index].Id} overlap.");
            }
        }

        var duplicatePatientSlots = activeAppointments
            .GroupBy(appointment => new { appointment.PatientId, appointment.DoctorId, appointment.Date })
            .Where(group => group.Count() > 1)
            .ToList();

        Assert.Empty(duplicatePatientSlots);
    }

    [Fact]
    public async Task DevelopmentSeed_DoesNotContainOldLocationData()
    {
        var cities = await JsonDataLoader.LoadJsonAsync<CitySeedMaterializer>("cities.json");
        var governorates = await JsonDataLoader.LoadJsonAsync<GovernorateSeedMaterializer>("governorates.json");
        var doctors = await LoadDoctors();

        Assert.DoesNotContain(cities, city => city.Name.Contains("Sohag", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(governorates, governorate =>
            governorate.Name.Contains("Sohag", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(doctors, doctor =>
            doctor.ClinicAddress.Contains("Sohag", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DevelopmentSeed_DoctorCalendarsAreUnambiguous()
    {
        var doctors = await LoadDoctors();

        foreach (var doctor in doctors)
        {
            Assert.Equal(doctor.Schedules.Count,
                doctor.Schedules.Select(schedule => schedule.DayOfWeek).Distinct().Count());
            Assert.Equal(doctor.ScheduleOverrides.Count,
                doctor.ScheduleOverrides.Select(overrideEntry => overrideEntry.Date).Distinct().Count());
            Assert.All(doctor.Schedules, schedule => Assert.True(schedule.StartTime < schedule.EndTime));
        }
    }

    private static async Task<List<DoctorSeedMaterializer>> LoadDoctors() =>
        await JsonDataLoader.LoadJsonAsync<DoctorSeedMaterializer>("doctors.json");

    private static async Task<List<PatientSeedMaterializer>> LoadPatients() =>
        await JsonDataLoader.LoadJsonAsync<PatientSeedMaterializer>("patients.json");

    private static async Task<List<AppointmentSeedMaterializer>> LoadAppointments() =>
        await JsonDataLoader.LoadJsonAsync<AppointmentSeedMaterializer>("appointments.json");
}
