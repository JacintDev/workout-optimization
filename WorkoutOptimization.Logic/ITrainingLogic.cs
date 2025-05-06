using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface ITrainingLogic
    {
        Task<bool> Create(TrainingDto entity, User user);
        void Delete(int id);
        (bool, Training?) GetIsActiveTraining(User user);
        Training Read(int id);
        IQueryable<Training> ReadAll();
        void Update(TrainingDto entity, int id);
    }
}