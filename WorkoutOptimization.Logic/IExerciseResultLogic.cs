using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface IExerciseResultLogic
    {
        Task CreateExerciseResult(ExerciseResultCreateModel model);
        Task<IEnumerable<ExerciseResultReturnedValueModel>> ReadAllById(int trainingid);
        Task<IEnumerable<ExerciseResultReturnedValueModel>> ReadAllByUser(User user);


    }
}