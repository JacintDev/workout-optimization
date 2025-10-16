using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IGyroscopeDataLogic
    {
        void Create(GyroscopeDataDto entity);
        void Delete(int id);
        GyroscopeData Read(int id);
        IQueryable<GyroscopeData> ReadAll();
        void Update(GyroscopeDataDto entity, int id);
    }
}