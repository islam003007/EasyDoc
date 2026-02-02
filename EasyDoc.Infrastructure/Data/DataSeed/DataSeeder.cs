using EasyDoc.Domain.Entities;
using EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDoc.Infrastructure.Data.DataSeed;

internal class DataSeeder
{
    public static async Task SeedAsync<TDomainType, TSeedMaterializer>(IServiceProvider serviceProvider, string fileName)
        where TDomainType : class
        where TSeedMaterializer : SeedMaterializerBase<TDomainType>
    {
        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            if (await context.Set<TDomainType>().AnyAsync())
                return; // already seeded

            var seedData = await JsonDataLoader.LoadJsonAsync<TSeedMaterializer>(fileName);
            var entities = seedData.Select(seed => seed.ToDomainObject());

            context.Set<TDomainType>().AddRange(entities);
            await context.SaveChangesAsync();
        }
    }
}