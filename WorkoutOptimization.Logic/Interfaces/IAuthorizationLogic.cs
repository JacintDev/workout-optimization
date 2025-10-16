using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IAuthorizationLogic
    {
        Task<(bool isAuthenticated, string Token, DateTime Expiration)> Login(LoginModel model);
        Task<bool> Register(RegisterModel model);
    }
}