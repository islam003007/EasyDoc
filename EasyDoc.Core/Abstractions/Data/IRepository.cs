using Ardalis.Specification;
using EasyDoc.Domain.Entities;

namespace EasyDoc.Application.Abstractions.Data;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{  
}