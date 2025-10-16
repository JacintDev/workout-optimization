using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IPromotionLogic
    {
        void Create(PromotionDto entity);
        void Delete(int id);
        Promotion Read(int id);
        IQueryable<Promotion> ReadAll();
        void Update(PromotionDto entity, int id);
    }
}