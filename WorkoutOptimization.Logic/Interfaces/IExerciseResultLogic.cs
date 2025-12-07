using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IExerciseResultLogic
    {
        Task CreateExerciseResult(ExerciseResultCreateModel model);
        Task<IEnumerable<ExerciseResultReturnedValueModel>> ReadAllById(int trainingid);
        Task<IEnumerable<ExerciseResultReturnedValueModel>> ReadAllByUser(User user);
        Task<int> GetUserIncorrectRepetitions(User user);
        Task<int> GetUserCorrectRepetitions(User user);


    }
}