using EasyDoc.Domain.Entities;

namespace EasyDoc.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, Guid entityId) : 
        base($"The {entity} with the ID {entityId} was not found", $"{entity}.NotFound", new {Entity = entity, EntityId = entityId})
    {
    }
}

public class NotFoundException<TEntity> : NotFoundException where TEntity : BaseEntity
{
    public static string EntityName = typeof(TEntity).Name;

    public NotFoundException(Guid entityId) : base(EntityName, entityId)
    {
    }
}
