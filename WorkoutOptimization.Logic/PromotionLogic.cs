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
    public class PromotionLogic : IPromotionLogic
    {
        readonly IMapper _mapper;
        readonly IRepository<Promotion> _repo;

        public PromotionLogic(IRepository<Promotion> repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public void Create(PromotionDto entity)
        {
            _repo.Create(_mapper.Map<Promotion>(entity));
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public Promotion Read(int id)
        {
            return _repo.Read(id);
        }

        public IQueryable<Promotion> ReadAll()
        {
            return _repo.ReadAll();
        }

        public void Update(PromotionDto entity, int id)
        {
            var ent = _mapper.Map<Promotion>(entity);
            ent.PromotionId = id;
            _repo.Update(ent);
        }


    }
}
