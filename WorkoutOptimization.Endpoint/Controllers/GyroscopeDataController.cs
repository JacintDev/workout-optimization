using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GyroscopeDataController : ControllerBase
    {
        readonly IGyroscopeDataLogic _logic;
        readonly IMapper _mapper;
        readonly UserManager<User> _userManager;


        public GyroscopeDataController(IGyroscopeDataLogic logic, IMapper mapper, UserManager<User> userManager)
        {
            _logic = logic;
            _mapper = mapper;
            _userManager = userManager;
        }


        //TODO csak a sajátodat adja vissza
        [HttpGet]
        public IEnumerable<GyroscopeData> Get()
        {
            return _logic.ReadAll();
        }

        // GET api/<GyroscopeDataController>/5
        [HttpGet("{id}")]
        public GyroscopeData Get(int id)
        {
            return _logic.Read(id);
        }

        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GyroscopeDataDto entity)
        {
            if (String.IsNullOrEmpty(this.User.Identity!.Name))
            {
                return BadRequest(new { message = "Unathorized!" });
            }
            var user = await _userManager.FindByEmailAsync(this.User.Identity.Name);
            if (user != null)
            {
                _logic.Create(entity, user);
                return Ok(new { message = "GyroscopeData created!" });
            }
            else{
                return BadRequest(new { message = "Unathorized!" });
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public void Update([FromBody] GyroscopeDataDto entity, int id)
        {
            _logic.Update(entity, id);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logic.Delete(id);
        }
    }
}
