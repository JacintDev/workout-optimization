using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface IAuthorizationLogic
    {
        Task<bool> Register(RegisterModel model);
    }
}