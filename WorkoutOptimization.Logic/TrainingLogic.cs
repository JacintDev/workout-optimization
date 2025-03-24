using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Models;
using WorkoutOptimization.Repository;

namespace WorkoutOptimization.Logic
{
    public class TrainingLogic : ITrainingLogic
    {
        readonly IMapper _mapper;
        readonly IRepository<Training> _repo;

        public TrainingLogic(IRepository<Training> repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public void Create(TrainingDto entity)
        {
            _repo.Create(_mapper.Map<Training>(entity));
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public Training Read(int id)
        {
            return _repo.Read(id);
        }

        public IQueryable<Training> ReadAll()
        {
            return _repo.ReadAll();
        }

        public void Update(TrainingDto entity, int id)
        {
            var ent = _mapper.Map<Training>(entity);
            ent.TrainingId = id;
            _repo.Update(ent);
        }


    }
}
