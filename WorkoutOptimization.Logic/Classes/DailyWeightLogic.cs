using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Logic.Interfaces;
using WorkoutOptimization.Models.Entities;
using WorkoutOptimization.Models.Models;
using WorkoutOptimization.Repository;

namespace WorkoutOptimization.Logic.Classes
{
    public class DailyWeightLogic : IDailyWeightLogic
    {
        IRepository<DailyWeight> _repo;
        IMapper _mapper;
        public DailyWeightLogic(IRepository<DailyWeight> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public void Create(DailyWeightCreateModel entity, User user)
        {
            var ent=_mapper.Map<DailyWeight>(entity);
            ent.User = user;
            user.Weight = entity.Weight; // TODO: update weight from int to float 
            _repo.Create(ent);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public DailyWeightViewModel Read(int id)
        {
            return _mapper.Map<DailyWeightViewModel>(_repo.Read(id));
        }

        public IQueryable<DailyWeightViewModel> ReadAll(bool role, string UserId)
        {
            if (!role)
            {
                return _mapper.ProjectTo<DailyWeightViewModel>(_repo.ReadAll().Where(x => x.UserId == UserId));
            }
            return _mapper.ProjectTo<DailyWeightViewModel>(_repo.ReadAll());
        }

        public void Update(DailyWeightCreateModel entity, int id)
        {
            var mapped = _mapper.Map<DailyWeight>(entity);
            mapped.DailyWeightId = id;
            _repo.Update(mapped);
        }
    }
}
