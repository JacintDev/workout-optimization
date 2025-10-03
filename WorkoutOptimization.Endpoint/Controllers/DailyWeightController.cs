using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class DailyWeightController : ControllerBase
    {
        readonly IDailyWeightLogic _logic;
        readonly UserManager<User> _userManager;
        public DailyWeightController(IDailyWeightLogic logic, UserManager<User> userManager)
        {
            _logic = logic;
            _userManager = userManager;
        }
        [HttpGet]
        public IEnumerable<DailyWeightViewModel> Get()
        {
            var role = User.IsInRole("Admin");
            var userId= User.FindFirst("UserId")?.Value;
            return _logic.ReadAll(role, userId);
        }

        // GET api/<DailyWeightController>/5
        [HttpGet("{id}")]
        public DailyWeightViewModel Get(int id)
        {
            return _logic.Read(id);
        }

        // POST api/<DailyWeightController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]DailyWeightCreateModel value)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
                _logic.Create(value, user!);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<DailyWeightController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]DailyWeightCreateModel value)
        {
            try
            {
            _logic.Update(value, id);
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        // DELETE api/<DailyWeightController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logic.Delete(id);
        }
    }
}
