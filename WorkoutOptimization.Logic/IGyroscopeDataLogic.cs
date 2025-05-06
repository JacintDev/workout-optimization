using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface IGyroscopeDataLogic
    {
        void Create(GyroscopeDataDto entity, User user);
        void Delete(int id);
        GyroscopeData Read(int id);
        IQueryable<GyroscopeData> ReadAll();
        void Update(GyroscopeDataDto entity, int id);
    }
}