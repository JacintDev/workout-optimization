using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

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

        [HttpGet]
        public async Task WriteToJsonFile(int id, bool valid)
        {

            var datas = _mapper.Map<List<GyroscopeDataDto>>(Get(id).GyroscopeData);

            var enriched = datas.Select(x => new
            {
                x.GyrosX,
                x.GyrosY,
                x.GyrosZ,
                x.AccelX,
                x.AccelY,
                x.AccelZ,
                IsValid = Get(id).IsCorrect
            }).ToList();

            var json = JsonConvert.SerializeObject(enriched);
            await System.IO.File.WriteAllTextAsync("adat.json", json);

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
        public async Task<IActionResult> UpdateTraining([FromBody] TrainingDto entity, int id)
        {
            if (String.IsNullOrEmpty(this.User.Identity!.Name))
            {
                throw new UnauthorizedAccessException("Unathorized!");
            }
            var user = await _userManager.FindByEmailAsync(this.User.Identity.Name);
            if (user == null) {
                throw new UnauthorizedAccessException("Unathorized!");
            }

            _logic.Update(entity, id, user);
            return Ok(new { message = "Training updated!" });

        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> StopTraining(int id)
        {
            if (String.IsNullOrEmpty(this.User.Identity!.Name))
            {
                throw new UnauthorizedAccessException("Unathorized!");
            }
            var user = await _userManager.FindByEmailAsync(this.User.Identity.Name);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Unathorized!");
            }
            _logic.StopTraining(id, user);
            return Ok(new { message = "Training stopped!" });
        }
        [Authorize]
        [HttpGet]
        public async Task<int> CountTrainings()
        {
            var user = await _userManager.FindByEmailAsync(this.User.Identity!.Name!);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Unathorized!");
            }
            return _logic.CountTrainings(user);
        }
        [HttpGet]
        public async Task<IQueryable<CountWorkoutSessionModel>> CountWorkoutSessions()
        {
            var user = await _userManager.FindByEmailAsync(this.User.Identity!.Name!);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Unathorized!");
            }
            var res=_logic.CountWorkoutSessions(user);
            return res;
        }


        [HttpGet]
        [Authorize]
        public async Task<string> GetLastTrainingDate()
        {
            var user = await _userManager.FindByEmailAsync(this.User.Identity!.Name!);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Unathorized!");
            }
            string res = _logic.GetLastTrainingDate(user).Value.ToShortDateString();
            return res;
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
