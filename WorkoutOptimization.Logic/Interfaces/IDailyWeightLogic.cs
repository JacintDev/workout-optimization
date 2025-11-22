using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IDailyWeightLogic
    {
        void Create(DailyWeightCreateModel entity, User userId);
        void Delete(int id);
        DailyWeightViewModel Read(int id);
        IQueryable<DailyWeightViewModel> ReadAll(bool role, string userId);
        void Update(DailyWeightCreateModel entity, int id);
        List<DailyWeightViewModel> GetUserWeights(string userId);
        List<DailyWeightViewModel> GetUserWeightsMonthly(string userId);
    }
}