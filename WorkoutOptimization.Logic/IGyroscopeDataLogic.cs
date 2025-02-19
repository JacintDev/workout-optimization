using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface IGyroscopeDataLogic
    {
        void Create(GyroscopeData entity);
        void Delete(int id);
        GyroscopeData Read(int id);
        IQueryable<GyroscopeData> ReadAll();
        void Update(GyroscopeData entity);
    }
}