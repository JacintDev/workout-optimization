using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        readonly IExerciseLogic _logic;
        readonly IMapper _mapper;
       

        public ExerciseController(IExerciseLogic logic, IMapper mapper)
        {
            _logic = logic;
            _mapper = mapper;
        }

       
        [HttpGet]
        public IEnumerable<Exercise> Get()
        {
            return _logic.ReadAll();
        }

        
        [HttpGet("{id}")]
        public Exercise Get(int id)
        {
            return _logic.Read(id);
        }

       
        [HttpPost]
        public void Post([FromBody] ExerciseDto entity)
        {
            _logic.Create(entity);
        }


        [HttpPut("{id}")]
        public void Update([FromBody] ExerciseDto entity, int id)
        {
            _logic.Update(entity, id);
        }

       
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logic.Delete(id);
        }
    }
}
