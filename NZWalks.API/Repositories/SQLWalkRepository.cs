using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;

        public SQLWalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null
                                                    , string? sortBy = null, bool isAscending = true
                                                    , int pageNumber = 1, int pageSize = 1000)
        {
            var walks = dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

            /* 필터링 */
            if(string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)     // 필터링할 Name 컬럼의 값이 있는지 확인
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))    //StringComparison.OrdinalIgnoreCase: 소문자, 대문자 구분 X
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));     //Name 컬럼 중 filterQuery값을 가지고 있는 레코드 조회
                }
                
            }

            /* 정렬 */
            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    // isAscending == true면 Name 컬럼 오름차순 정렬, 아니면 Name 컬럼 내림차순 정렬
                    walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    // isAscending == true면 LengthInKm 컬럼 오름차순 정렬, 아니면 LengthInKm 컬럼 내림차순 정렬
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }

            /* 페이징 */
            var skipResults = (pageNumber - 1) * pageSize;  // skip할 레코드 수

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();  // 레코드 skipResults개 스킵하고 pageSize개 가져오기

            // Include("모델명") 또는 Include(x => x.모델명): 참조하는 도메인 모델도 함께 가져오기
            //return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
        }

        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();

            return walk;
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var existWalk = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

            if(existWalk == null)
            {
                return null;
            }

            existWalk.Name = walk.Name;
            existWalk.Description = walk.Description;
            existWalk.RegionId = walk.RegionId;
            existWalk.DifficultyId = walk.DifficultyId;

            await dbContext.SaveChangesAsync();

            return existWalk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var existWalk = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

            if (existWalk == null)
            {
                return null;
            }

            dbContext.Walks.Remove(existWalk);
            await dbContext.SaveChangesAsync();

            return existWalk;
        }

    }
}
