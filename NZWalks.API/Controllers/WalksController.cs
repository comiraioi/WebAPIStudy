using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models;
using NZWalks.API.Repositories;
using NZWalks.API.CustomActionFilters;

namespace NZWalks.API.Controllers
{

    // https://localhost:7202/api/Walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(NZWalksDbContext dbContext, IMapper mapper, IWalkRepository walkRepository)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        #region GetAll

        // Get All (필터링, 정렬, 페이징)
        // https://localhost:7202/api/Walks
        // ?FilterOn=필터링하고싶은컬럼명&filterQauery=값&sortBy=정렬기준컬럼명&isAscending=true&pageNumber=n번째페이지&pageSize=한페이지당출력할레코드수
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
                                                [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
                                                [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            // 도메인 모델의 모든 데이터 가져오기
            var walksDomain = await walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true   // isAscending이 null이면 true 할당
                                                                , pageNumber, pageSize); 

            // 도메인 모델을 DTO로 매핑
            var walksDto = mapper.Map<List<WalkDto>>(walksDomain);

            return Ok(walksDto);
        }

        #endregion

        #region Create

        // CREATE: https://localhost:7202/api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            // DTO를 도메인 모델로 매핑
            var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

            walkDomainModel = await walkRepository.CreateAsync(walkDomainModel);

            // 도메인 모델을 DTO로 매핑
            var walkDto = mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);
        }

        #endregion

        #region GetById

        // Get Walk: https://localhost:7202/api/walks/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // id로 도메인 모델의 레코드 가져오기
            var walkDomainModel = await walkRepository.GetByIdAsync(id);
            
            if(walkDomainModel == null)
            {
                return NotFound();
            }

            // 도메인 모델을 DTO로 매핑
            var walkDto = mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);
        }

        #endregion

        #region Update

        // Update: https://localhost:7202/api/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);

            walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);

            if (walkDomainModel == null)
            {
                return NotFound();
            }

            var walkDto = mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);
        }

        #endregion

        #region Delete
        // Delete: https://localhost:7202/api/walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walk = await walkRepository.DeleteAsync(id);

            if(walk == null)
            {
                return NotFound();
            }

            return Ok();
        }

        #endregion

    }
}
