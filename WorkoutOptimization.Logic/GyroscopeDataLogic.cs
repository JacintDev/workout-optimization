using WorkoutOptimization.Models;
using WorkoutOptimization.Repository;

namespace WorkoutOptimization.Logic
{
    public class GyroscopeDataLogic : IGyroscopeDataLogic
    {
        readonly IRepository<GyroscopeData> _repo;

        public void Create(GyroscopeData entity)
        {
            _repo.Create(entity);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public GyroscopeData Read(int id)
        {
            return _repo.Read(id);
        }

        public IQueryable<GyroscopeData> ReadAll()
        {
            return _repo.ReadAll();
        }

        public void Update(GyroscopeData entity)
        {
            _repo.Update(entity);
        }
    }
}
