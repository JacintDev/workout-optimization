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

        public void Create(GyroscopeDataDto entity, User user)
        {
            var gyroscopeData = _mapper.Map<GyroscopeData>(entity);
            gyroscopeData.User = user;
            gyroscopeData.UserId = user.Id;
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
