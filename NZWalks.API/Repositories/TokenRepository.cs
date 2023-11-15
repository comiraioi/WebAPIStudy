using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NZWalks.API.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;

        public TokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public string CreateJWTToken(IdentityUser user, List<string> roles)
        {
            // 클레임(토큰에 담기는 정보) 생성
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, user.Email));    //사용자 아이덴티티

            // 권한(roles)
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 대칭키
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));   //appsettings.json에서 설정한 키

            // 크리덴셜:  사용자 아이디와 패스워드 정보를 토대로 생성된 보안 토큰, 초기 인증 이후 크리덴셜 확인만으로 사용자 인증을 대신할 수 있음
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),   // 토큰 유지 시간 15분
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
