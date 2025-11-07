using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ScottPlot.Renderable;
using WorkoutOptimization.Endpoint.Helpers;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PulseController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IPulseLogic _pulseLogic;
        private readonly IHubContext<ExerciseHub> _hubContext;

        public PulseController(UserManager<User> userManager, IPulseLogic pulseLogic, IHubContext<ExerciseHub> hubContext)
        {
            _userManager = userManager;
            _pulseLogic = pulseLogic;
            _hubContext = hubContext;
        }

        // GET: api/<PulseController>
        //[HttpGet]
        //public IActionResult Get()
        //{

        //}

        // GET api/<PulseController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PulseController>
        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] PulseSendModel pulse)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var res = _pulseLogic.CompareToRestPulse(user, (int)pulse.Pulse);
            await _hubContext.Clients.All.SendAsync("ReceivePulse", res.Item2);
            return Ok(new
            {
                message = res.Item1,
                pulse = res.Item2
            });


        }

        // PUT api/<PulseController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<PulseController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
