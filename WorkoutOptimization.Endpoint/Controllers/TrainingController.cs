using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic;
using WorkoutOptimization.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        readonly ITrainingLogic _logic;
        readonly IMapper _mapper;

    
        public TrainingController(ITrainingLogic logic, IMapper mapper)
        {
            _logic = logic;
            _mapper = mapper;
        }


        [HttpGet]
        public IEnumerable<Training> Get()
        {
            return _logic.ReadAll();
        }


        [HttpGet("{id}")]
        public Training Get(int id)
        {
            return _logic.Read(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post([FromBody] TrainingDto entity)
        {
            _logic.Create(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public void Update([FromBody] TrainingDto entity, int id)
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
