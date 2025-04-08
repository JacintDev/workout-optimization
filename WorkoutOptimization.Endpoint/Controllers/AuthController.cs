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
            var result = _logic.Login(model);
            if (result.Result.isAuthenticated) { 
                return Ok(new {token=result.Result.Token, expiration = result.Result.Expiration});
            }
            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> IsLoggedIn()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return BadRequest(new { isLoggedIn = false });
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user != null)
            {
                return (Ok(new { isLoggedIn = true, user = _mapper.Map<UserViewModel>(user) }));
            }
            else
            {
                return (BadRequest(new { isLoggedIn = false }));
            }
        }

    }
}
