using NZWalks.API.Models.Domain;

namespace NZWalks.API.Models.DTO
{
    internal class WalkDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double LengthInKm { get; set; }

        public string? WalkImageUrl { get; set; }

        /* 도메인 모델을 참조하여 따로 컬럼 설정하지 않아도 됨
        public Guid DifficultyId { get; set; }

        public Guid RegionId { get; set; } */

        public DifficultyDto Difficulty { get; set; }

        public RegionDto Region { get; set; }
    }
}