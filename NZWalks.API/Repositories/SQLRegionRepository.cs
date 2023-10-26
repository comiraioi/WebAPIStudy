using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext dbContext;

        //생성자 단축어 ctor
        public SQLRegionRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //상속받은 리포지토리명 선택 후 ctrl+. 누르면 자동으로 인터페이스 구현됨
        public async Task<List<Region>> GetAllAsync()   //program.cs 파일에 주입해야함
        {
            return await dbContext.Regions.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
           return await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region> CreateAsync(Region region)
        {
            await dbContext.Regions.AddAsync(region);
            await dbContext.SaveChangesAsync();

            return region;
        }

        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            var existRegion = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (existRegion == null)
            {
                return null;
            }

            existRegion.Code = region.Code;
            existRegion.Name = region.Name;
            existRegion.RegionImageUrl = region.RegionImageUrl;

            await dbContext.SaveChangesAsync();

            return existRegion;
        }


        public async Task<Region?> DeleteAsync(Guid id)
        {
            var existRegion = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (existRegion == null)
            {
                return null;
            }

            dbContext.Regions.Remove(existRegion);  //Remove는 비동기 메서드가 아니므로 await 사용 불가
            await dbContext.SaveChangesAsync();

            return existRegion;
        }


    }
}
