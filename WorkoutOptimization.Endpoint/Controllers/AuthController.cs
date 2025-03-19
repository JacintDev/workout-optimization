using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        UserManager<User> _userManager;
        IMapper _mapper;
        IAuthorizationLogic _logic;
        public AuthController(UserManager<User> userManager, IMapper mapper, IAuthorizationLogic logic)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logic = logic;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result= await _logic.Register(model);

            if (result)
            {
                //return ok with message json format
                return Ok(new { message = "User created successfully" });
            }
            else
            {
                //return bad request with message json format
               throw new Exception("User creation failed.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user= await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var claim = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.Email) };
                foreach (var role in await _userManager.GetRolesAsync(user))
                {
                    claim.Add(new Claim(ClaimTypes.Role, role));
                }
                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("nagyonhosszutitkoskodhelyenagyonhosszutitkoskodhelye"));
                var token = new JwtSecurityToken(
                issuer: "http://www.security.org", audience: "http://www.security.org",
                claims: claim, expires: DateTime.Now.AddMinutes(180),
                signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}
