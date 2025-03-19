using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                return BadRequest(new { message = "User creation failed" });
            }

        }
    }
}
