using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkoutOptimization.Endpoint.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ExerciseResultController : ControllerBase
    {
        readonly IExerciseResultLogic _exerciseResultLogic;
        readonly UserManager<User> _userManager;
        public ExerciseResultController(IExerciseResultLogic exerciseResultLogic, UserManager<User> userManager)
        {
            _exerciseResultLogic = exerciseResultLogic;
            _userManager = userManager;
        }


        // GET: api/<ExerciseResultController>
        [HttpGet("{id}")]
        public async Task<IEnumerable<ExerciseResultReturnedValueModel>> GetAllById(int id)
        {
            return await _exerciseResultLogic.ReadAllById(id);
        }

        [HttpGet]
        public async Task<IEnumerable<ExerciseResultReturnedValueModel>> GetAllByUser()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid user");
            }
            return await _exerciseResultLogic.ReadAllByUser(user);
        }




    }
}
