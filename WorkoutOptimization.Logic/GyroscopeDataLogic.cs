using AutoMapper;
using WorkoutOptimization.Models;
using WorkoutOptimization.Repository;

namespace WorkoutOptimization.Logic
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

            _repo.Create(_mapper.Map<GyroscopeData>(entity));
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
