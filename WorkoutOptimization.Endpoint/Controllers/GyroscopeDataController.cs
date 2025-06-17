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
       // readonly UserManager<User> _userManager;


        public GyroscopeDataController(IGyroscopeDataLogic logic, IMapper mapper)
        {
            _logic = logic;
            _mapper = mapper;
           
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
        public IActionResult Post([FromBody] GyroscopeDataDto entity)
        {

            try
            {
                _logic.Create(entity);
                return Ok(new { message = "GyroscopeData created!" });
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }
               

        }

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
