using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]")]
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
        public IEnumerable<Training> Get()
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
        public async Task<IActionResult> Post([FromBody] TrainingDto entity)
        {
            //_logic.Create(entity);

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
        public void Update([FromBody] TrainingDto entity, int id)
        {
            _logic.Update(entity, id);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logic.Delete(id);
        }
    }
}
