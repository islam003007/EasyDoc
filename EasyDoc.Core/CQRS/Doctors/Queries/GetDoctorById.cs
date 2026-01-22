using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Queries.Common;
using EasyDoc.Application.Errors;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Doctors.Queries;

public record GetDoctorByIdQuery(Guid DoctorId) : IQuery<DoctorResponse>;

internal class GetDoctorByIdQueryValidator : AbstractValidator<GetDoctorByIdQuery>
{
    public GetDoctorByIdQueryValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();
    }
}

internal class GetDoctorByIdQueryHandler : IQueryHandler<GetDoctorByIdQuery, DoctorResponse>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetDoctorByIdQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Result<DoctorResponse>> HandleAsync(GetDoctorByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DoctorDetails
            .Where(d => d.Id == query.DoctorId)
            .Select(DoctorResponseMapper.ToDoctorResponse)
            .FirstOrDefaultAsync() ??
            Result.Failure<DoctorResponse>(DoctorErrors.NotFound(query.DoctorId));
    }
}