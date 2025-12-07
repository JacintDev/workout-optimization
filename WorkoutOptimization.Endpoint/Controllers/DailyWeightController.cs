using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
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
        [HttpGet]
        public async Task<IActionResult> GetUserWeights()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            var weights = _logic.GetUserWeights(user!.Id);
            return Ok(weights);
        }
        [HttpGet]
        public async Task<IActionResult> GetUserMonthlyWeights()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            var weights = _logic.GetUserWeightsMonthly(user!.Id);
            return Ok(weights);
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

        [HttpGet]
        public async Task<IActionResult> IsSettedUpDailyWeight()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            var res = _logic.IsSettedUpDailyWeight(user!.Id);
            return Ok(res);
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
