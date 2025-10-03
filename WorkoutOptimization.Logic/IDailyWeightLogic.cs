using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface IDailyWeightLogic
    {
        void Create(DailyWeightCreateModel entity, string userId);
        void Delete(int id);
        DailyWeight Read(int id);
        IQueryable<DailyWeight> ReadAll(bool role, string userId);
        void Update(DailyWeightCreateModel entity, int id);
    }
}