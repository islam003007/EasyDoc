namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal abstract class SeedMaterializerBase<T>
{
    public Guid Id { get; set; }

    public abstract T ToDomainObject();

    protected void SetDomainObjectId(T domainObject)
    {
        var prop = typeof(T).GetProperty("Id")
            ?? throw new InvalidOperationException($"Id property not found on {typeof(T).Name}");
        prop.SetValue(domainObject, Id);
    }
}