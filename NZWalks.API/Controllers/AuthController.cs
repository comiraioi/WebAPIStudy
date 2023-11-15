using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Models;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly NZWalksAuthDbContext dbContext;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }


        #region Registor

        // POST: /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                //가입한 사용자 권한 설정
                if(registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    if(identityResult.Succeeded)
                    {
                        return Ok("Register succeed! Please login.");
                        
                    }
                }
                
            }

            return BadRequest("Register failed.");
        }

        #endregion

        #region Login

        // POST: /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            // username 확인
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

            if (user == null) {
                return BadRequest("Username incorrect");
            }

            var passwordCheckResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (passwordCheckResult)    // true
            {
                // 권한 목록 가져오기
                var roles = await userManager.GetRolesAsync(user);

                if (roles != null)
                {
                    // Token 생성
                    var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());

                    var response = new LoginResponseDto     // 토큰 정보
                    {
                        jwtToken = jwtToken
                    };

                    return Ok(response);
                }

                return BadRequest("Failed to create Token.");
            }

            return BadRequest("Password incorrect");

        }

        #endregion
    }
}
