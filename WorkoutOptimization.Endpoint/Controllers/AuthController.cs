using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

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

            try
            {
                var result = await _logic.Register(model);
                return Ok(new { message = "User created successfully" });
            }
            catch (Exception e)
            {
                return BadRequest($"Failed to register {e.Message}");
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

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            user = _mapper.Map(model, user);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "User updated successfully" });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

    }
}
