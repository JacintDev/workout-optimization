using AutoMapper;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;
using WorkoutOptimization.Repository;

namespace WorkoutOptimization.Logic.Classes
{
    public class GyroscopeDataLogic : IGyroscopeDataLogic
    {
        readonly IMapper _mapper;
        readonly IRepository<GyroscopeData> _repo;

        public GyroscopeDataLogic(IRepository<GyroscopeData> repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public void Create(GyroscopeDataDto entity)
        {
            var gyroscopeData = _mapper.Map<GyroscopeData>(entity);
            gyroscopeData.Date = DateTime.Now;
            _repo.Create(gyroscopeData);
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

        public void Update(GyroscopeDataDto entity, int id)
        {
            var ent = _mapper.Map<GyroscopeData>(entity);
            ent.GyroscopeDataId = id;
            _repo.Update(ent);
        }
    }
}
