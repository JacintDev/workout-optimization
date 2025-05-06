using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        readonly ITrainingLogic _logic;
        readonly IMapper _mapper;
        readonly UserManager<User> _userManager;

    
        public TrainingController(ITrainingLogic logic, IMapper mapper, UserManager<User> userManager)
        {
            _logic = logic;
            _mapper = mapper;
            _userManager = userManager;
        }


        //Csak a sajátodat adja vissza, kivéve admin
        [Authorize]
        [HttpGet]
        public IEnumerable<Training> GetAll()
        {
            return _logic.ReadAll();
        }

        [Authorize]
        [HttpGet("{id}")]
        public Training Get(int id)
        {
            return _logic.Read(id);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTraining([FromBody] TrainingDto entity)
        {
            

            if (String.IsNullOrEmpty(this.User.Identity!.Name))
            {
                return BadRequest(new { message = "Unathorized!"});
            } 
            var user= await _userManager.FindByEmailAsync(this.User.Identity.Name);
            if (user == null) {
                return BadRequest(new { message = "Unathorized!" });
            }
            var res= await _logic.Create(entity, user);
            if (res)
            {
                return Ok(new { message = "Training created!" });
            }
            else
            {
                return BadRequest(new { message = "Training create has been failed" });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public void UpdateTraining([FromBody] TrainingDto entity, int id)
        {
            _logic.Update(entity, id);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public void DeleteTraining(int id)
        {
            _logic.Delete(id);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetActiveTraining()
        {
            if (String.IsNullOrEmpty(this.User.Identity!.Name))
            {
                return BadRequest(new { message = "Unathorized!" });
            }
            var user = await _userManager.FindByEmailAsync(this.User.Identity.Name);
            if (user == null)
            {
                return BadRequest(new { message = "Unathorized!" });
            }
            //send to logic
        
            var res = _logic.GetIsActiveTraining(user);
            if (res.Item1!=false)
            {
                return Ok(new { UserId= res.Item2.UserId, Start= res.Item2.Start, TrainingId= res.Item2.TrainingId});
            }
            else
            {
                return NotFound(new { message = "No active training found" });
            }
        }
    }
}
