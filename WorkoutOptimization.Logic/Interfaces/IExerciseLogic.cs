using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IExerciseLogic
    {
        void Create(ExerciseDto entity);
        void Delete(int id);
        Exercise Read(int id);
        IQueryable<Exercise> ReadAll();
        void Update(ExerciseDto entity, int id);
    }
}