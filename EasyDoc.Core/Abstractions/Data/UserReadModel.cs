namespace EasyDoc.Application.Abstractions.Data;

// A little hack to not depend on Identity, instead of the application layer depending on Identity it just depends on this readmodel.
// This model is mapped to a view in the database.
// THIS IS ONLY USED FOR READONLY QUIRIES
public class UserReadModel
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
};
