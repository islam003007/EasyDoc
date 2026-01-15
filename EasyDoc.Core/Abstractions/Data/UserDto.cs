namespace EasyDoc.Application.Abstractions.Data;

// A little hack to not depend on Identity, instead of the application layer depending on Identity it just depends on this minimal dto.
// an alternative would be to include them as part of the domain. or to just create a view which would introduce some coupling to identity
// THIS IS ONLY USED FOR READONLY QUIRIES
public record UserDto(Guid UserId, string Email);
