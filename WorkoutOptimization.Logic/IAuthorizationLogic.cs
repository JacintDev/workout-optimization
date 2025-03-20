using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface IAuthorizationLogic
    {
        Task<(bool isAuthenticated, string Token, DateTime Expiration)> Login(LoginModel model);
        Task<bool> Register(RegisterModel model);
    }
}