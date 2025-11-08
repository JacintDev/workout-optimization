using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IPulseLogic
    {
        PulseViewModel CompareToRestPulse(User user, int pulse);
    }
}