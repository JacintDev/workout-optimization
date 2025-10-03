using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public interface IDailyWeightLogic
    {
        void Create(DailyWeightCreateModel entity, string userId);
        void Delete(int id);
        DailyWeightViewModel Read(int id);
        IQueryable<DailyWeightViewModel> ReadAll(bool role, string userId);
        void Update(DailyWeightCreateModel entity, int id);
    }
}