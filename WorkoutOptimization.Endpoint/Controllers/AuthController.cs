using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Models;

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Weight = model.Weight,
                Height = model.Height,
                Level = model.Level
            };
        }
    }
}
