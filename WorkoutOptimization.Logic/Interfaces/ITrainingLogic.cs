using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface ITrainingLogic
    {
        Task<bool> Create(TrainingDto entity, User user);
        void Delete(int id);
        (bool, Training?) GetIsActiveTraining(User user);
        Training Read(int id);
        void StopTraining(int id, User user);
        IQueryable<Training> ReadAll();
        void Update(TrainingDto entity, int id, User user);
        int CountTrainings(User user);
        IQueryable<CountWorkoutSessionModel> CountWorkoutSessions(User user);
        DateTime? GetLastTrainingDate(User user);
    }
}