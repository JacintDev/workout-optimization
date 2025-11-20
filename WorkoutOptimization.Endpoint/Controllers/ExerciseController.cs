using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("[controller]")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post([FromBody] ExerciseDto entity)
        {
            _logic.Create(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public void Update([FromBody] ExerciseDto entity, int id)
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
