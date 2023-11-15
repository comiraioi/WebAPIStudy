using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:7202/api/Regions   
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        //매개변수 클릭 후 ctrl+. 누르면 자동으로 필드 생성 및 할당
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        //DbContext, IRegionRepository의 인터페이스, IMapper의 AutoMApper 주입
        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)    
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        #region GetAll

        //GET: https://localhost:7202/api/regions
        [HttpGet]
        [Authorize(Roles = "Reader")]     // 인증한 사람만 접근할 수 있음 (비인증 접근시 401에러 발생)
        public async Task<IActionResult> GetAll()   //async: 비동기식 메서드 (병렬적으로 태스크 수행)
        {
            /*var regions = new List<Region>
            {
                new Region
                {
                    Id = Guid.NewGuid(),
                    Name = "Auckland Region",
                    Code = "AKL",
                    RegionImageUrl = "https://www.pexels.com/photo/black-and-white-17829466/"
                },
                new Region
                {
                    Id = Guid.NewGuid(),
                    Name = "Wellington Region",
                    Code = "WLG",
                    RegionImageUrl = "https://www.pexels.com/photo/beach-at-dusk-17829445/"
                }
            };*/

            
            /* //DB에서 가져온 도메인 모델
            //var regionsDomain = dbContext.Regions.ToList();
            var regionsDomain = await dbContext.Regions.ToListAsync();  //비동기식 호출*/

            // 비동기식으로 도메인 모델 가져오는 인터페이스 호출
            var regionsDomain = await regionRepository.GetAllAsync();


            /* 도메인 모델을 DTO(Data Transfer Object)로 매핑
            var regionsDto = new List<RegionDto>();
            foreach (var region in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl
                });
            }*/

            // AutoMapper를 사용해 도메인 모델을 DTO로 매핑
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            return Ok(regionsDto);
        }

        #endregion


        #region GetById

        //GET: https://localhost:7202/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            /*
            // id로 도메인모델의 레코드 가져오기
            //var region = dbContext.Regions.Find(id)   // 매개변수가 PK인 경우만 사용
            //var region = dbContext.Regions.FirstOrDefault(x => x.Id == id);
            var region = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);*/
            var region = await regionRepository.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            /*
            //도메인 모델을 DTO로 매핑
            var regionDto = new RegionDto
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            }; */

            //return Ok(region);

            // AutoMapper를 사용해 도메인 모델을 DTO로 매핑하여 return
            return Ok(mapper.Map<RegionDto>(region));
        }

        #endregion


        #region Create

        //POST: https://localhost:7202/api/regions
        [HttpPost]
        [ValidateModel]     // CustomActionFilter 사용해 유효성 검사 코드 생략 가능: if (ModelState.IsValid){~쿼리작업~} else{return BadRequest(ModelState);}
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            /*
            // Dto를 도메인 모델로 매핑
            var regionDomainModel = new Region
            {
                Id = Guid.NewGuid(),
                Code = addRegionRequenstDto.Code,
                Name = addRegionRequenstDto.Name,
                RegionImageUrl = addRegionRequenstDto.RegionImageUrl
            }; */

            // AutoMapper를 사용해 DTO를 도메인 모델로 매핑
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            /*
            // 도메인 모델을 DB에 Insert
            //dbContext.Regions.Add(regionDomainModel);
            await dbContext.Regions.AddAsync(regionDomainModel)

            //dbContext.SaveChanges();    //commit
            await dbContext.SaveChangesAsync();;*/

            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            /*
            // 도메인 모델을 Dto로 매핑
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            */

            // AutoMapper를 사용해 도메인 모델을 DTO로 매핑
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            // POST 방식은 200이 아닌 201 또는 202를 반환 (location)
            // CreatedAtAction(nameof(액션명), new { 매개변수명 = 값 }, 모델)
            return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.Id }, regionDto);
        }

        #endregion


        #region Update

        //PUT: https://localhost:7202/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            /*
            //id로 도메인모델의 레코드 가져오기
            var region = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if(region == null)
            {
                return NotFound();
            }

            // Dto를 도메인 모델로 매핑 (Update)
            region.Code = updateRegionRequestDto.Code;
            region.Name = updateRegionRequestDto.Name;
            region.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            await dbContext.SaveChangesAsync();*/

            /* Dto를 도메인 모델로 매핑
            var region = new Region
            {
                Code = updateRegionRequestDto.Code,
                Name = updateRegionRequestDto.Name,
                RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            }; */

            // AutoMapper를 사용해 DTO를 도메인 모델로 매핑
            var region = mapper.Map<Region>(updateRegionRequestDto);

            region = await regionRepository.UpdateAsync(id, region);

            if (region == null)
            {
                return NotFound();
            }

            /*
            // 도메인 모델을 Dto로 매핑
            var regionDto = new RegionDto
            {
                Id = region.Id,
                Code = region.Code,
                Name = region.Name,
                RegionImageUrl = region.RegionImageUrl
            };
            */

            //return Ok(regionDto);

            // AutoMapper를 사용해 도메인 모델을 DTO로 매핑하여 return
            return Ok(mapper.Map<RegionDto>(region));
        }

        #endregion


        #region Delete

        //Delete: https://localhost:7202/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            /*
            //id로 도메인모델의 레코드 가져오기
            var region = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (region == null)
            {
                return NotFound();
            }

            // 도메인 모델의 레코드 삭제
            dbContext.Regions.Remove(region);
            await dbContext.SaveChangesAsync();
            */

            var region = await regionRepository.DeleteAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            return Ok();
        }

        #endregion
    }
}
