using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface IExerciseResultLogic
    {
        Task CreateExerciseResult(ExerciseResultCreateModel model);
    }
}