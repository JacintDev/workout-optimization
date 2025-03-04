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
         IGyroscopeDataLogic _logic;

        public GyroscopeDataController(IGyroscopeDataLogic logic)
        {
            _logic = logic;
        }

       
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

        // POST api/<GyroscopeDataController>
        [HttpPost]
        public void Post([FromBody] GyroscopeData entity)
        {
            _logic.Create(entity);
        }

        // PUT api/<GyroscopeDataController>/5

        [HttpPut]
        public void Update([FromBody] GyroscopeData entity)
        {
            _logic.Update(entity);
        }

        // DELETE api/<GyroscopeDataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logic.Delete(id);
        }
    }
}
