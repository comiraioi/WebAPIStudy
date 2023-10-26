using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public interface IWalkRepository
    {
        //filterOn: 필터링할 컬럼명 / filterQuery: 값 / sortBy: 정렬기준컬럼 / isAscending: 오름차순
        Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null
                                    , string? sortBy = null, bool isAscending = true
                                    , int pageNumber = 1, int pageSize = 1000);  

        Task<Walk> CreateAsync(Walk walk);

        Task<Walk?> GetByIdAsync(Guid id);

        Task<Walk?> UpdateAsync(Guid id, Walk walk);

        Task<Walk?> DeleteAsync(Guid id);
    }
}