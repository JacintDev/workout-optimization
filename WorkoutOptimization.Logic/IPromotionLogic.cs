using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
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