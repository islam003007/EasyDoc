using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.Infrastructure.Services.DataNormalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EasyDoc.Infrastructure.Data.Interceptors;

public class AddNormalizedNameAndPhoneticKeysInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        AddNormalizedNammeAndPhoneticKeys(dbContext);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddNormalizedNammeAndPhoneticKeys(DbContext? dbContext)
    {
        if (dbContext is null)
            return;

        var doctors = dbContext.ChangeTracker.Entries<Doctor>()
                               .Where(e => e.Property(d => d.PersonName).IsModified || e.State == EntityState.Added)
                               .Select(e => e?.Entity);

       foreach (var doctor in doctors)
        {
            if (doctor is null)
                continue;

            string name = doctor.PersonName;

            string normalizedName = ArabicNormalizer.Normalize(name.ToLower());

            string metaphoneKeys = String.Join(" ", DoubleMetaphone.GetKeys(name).Where(key => !String.IsNullOrEmpty(key)));

            doctor.SetNormalizedName(normalizedName);
            doctor.SetMetaphoneKeys(metaphoneKeys);
        }
    }
}
