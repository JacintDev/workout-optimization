using WorkoutOptimization.Models.Entities;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IPulseLogic
    {
        (string, int) CompareToRestPulse(User user, int pulse);
    }
}