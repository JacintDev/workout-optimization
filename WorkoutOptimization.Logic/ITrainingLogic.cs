using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface ITrainingLogic
    {
        void Create(TrainingDto entity);
        void Delete(int id);
        Training Read(int id);
        IQueryable<Training> ReadAll();
        void Update(TrainingDto entity, int id);
    }
}