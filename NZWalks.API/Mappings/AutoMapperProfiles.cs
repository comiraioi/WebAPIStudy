using AutoMapper;
using NZWalks.API.Models.DTO;
using NZWalks.API.Models.Domain;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace NZWalks.API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Region 매핑

            /* 컨트롤러에서 도메인 모델을 DTO(Data Transfer Object)로 매핑
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

            // Nuget 패키지 관리자에서 AutoMapper 설치
            // 도메인 모델을 DTO로 매핑
            CreateMap<Region, RegionDto>().ReverseMap();

            /*
            CreateMap<Region, RegionDto>()
                .ForMemeber(x => x. Id, opt => opt.MapFrom(x => x.DtoId))   //생략 가능
                .ReverseMap();
            */

            // Insert할 DTO를 도메인 모델로 매핑
            CreateMap<AddRegionRequestDto, Region>().ReverseMap();

            // Update할 DTO를 도메인 모델로 매핑
            CreateMap<UpdateRegionRequestDto, Region>().ReverseMap();

            #endregion

            #region Walk 매핑
            
            CreateMap<Walk, WalkDto>().ReverseMap();
            CreateMap<AddWalkRequestDto, Walk>().ReverseMap();
            CreateMap<UpdateWalkRequestDto, Walk>().ReverseMap();

            #endregion

            #region Difficulty 매핑

            CreateMap<Difficulty, DifficultyDto>().ReverseMap();

            #endregion

        }
    }
}
