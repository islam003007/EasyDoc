using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Doctors.Queries.Common;
using EasyDoc.Infrastructure.Data;
using EasyDoc.Infrastructure.Services.DataNormalization;
using EasyDoc.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Infrastructure.Services;

internal class DoctorSearchService : IDoctorSearchService
{
    private readonly ApplicationDbContext _context;

    public DoctorSearchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<DoctorSearchReadModel>>> SearchDoctorsAsync(
        string SearchInput,
        Guid? cityId = null,
        Guid? departmentId = null,
        int pageNumber = 1,
        int pageSize = PageConstants.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        // 1. Sanitize the input
        SearchInput = InputSanitizer.Sanitize(SearchInput); // also does trimming and removes consequetive whitespaces

        if (string.IsNullOrWhiteSpace(SearchInput))
        {
            return Result.Success<IEnumerable<DoctorSearchReadModel>>(Enumerable.Empty<DoctorSearchReadModel>());
        }

        // 2. Normalize (Lower case + Arabic normalization)
        SearchInput = ArabicNormalizer.Normalize(SearchInput.ToLower());

        // 3. Construct Metaphone Search Tokens
        IEnumerable<string> PhoneticSearchTokens = DoubleMetaphone.GetKeys(SearchInput).Where(x => !String.IsNullOrWhiteSpace(x));

        // 4. Construct the Lexical Search Tokens
        IEnumerable<string> LexicalSearchTokens = SearchInput
                                            .Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                            .Select(w => $"\"{w}\"");

        // 5. Construct the Search Terms
        string LexicalSearchTerm = String.Join(" NEAR ", LexicalSearchTokens);
        string PhoneticSearchTerms = String.Join(" OR ", PhoneticSearchTokens.Select(k => $"\"{k}*\""));

        if (PhoneticSearchTerms == String.Empty)
        {
            PhoneticSearchTerms = "\"___NULL___\"";
        }

        //6. Excute the Search SP

        var SearchResult = await _context.Database
            .SqlQuery<DoctorSearchReadModel>($@"
                                        EXEC dbo.SearchDoctorsByRankAndPaginate
                                            @LexicalSearchTerm = {LexicalSearchTerm}, 
                                            @PhoneticSearchTerms = {PhoneticSearchTerms},           
                                            @CityId = {cityId},                           
                                            @DepartmentId = {departmentId},                     
                                            @PageNumber = {pageNumber},                         
                                            @PageSize = {pageSize};")
                                            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<DoctorSearchReadModel>>(SearchResult);
    }
}
