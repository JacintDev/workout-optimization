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




        // POST api/<PulseController>
        [HttpPost()]
        public async Task<PulseViewModel> Post([FromBody] PulseSendModel pulse)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            PulseViewModel res = _pulseLogic.CompareToRestPulse(user, (int)pulse.Pulse);
            await _hubContext.Clients.All.SendAsync("ReceivePulse", res);
            return res;

        }





    }
}
