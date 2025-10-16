using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        readonly IPromotionLogic _logic;
        readonly IMapper _mapper;

    
        public PromotionController(IPromotionLogic logic, IMapper mapper)
        {
            _logic = logic;
            _mapper = mapper;
        }


        [HttpGet]
        public IEnumerable<Promotion> Get()
        {
            return _logic.ReadAll();
        }


        [HttpGet("{id}")]
        public Promotion Get(int id)
        {
            return _logic.Read(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post([FromBody] PromotionDto entity)
        {
            _logic.Create(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public void Update([FromBody] PromotionDto entity, int id)
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
