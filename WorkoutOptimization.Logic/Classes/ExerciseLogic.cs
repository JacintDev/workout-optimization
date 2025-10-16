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
    public class ExerciseLogic : IExerciseLogic
    {
        readonly IMapper _mapper;
        readonly IRepository<Exercise> _repo;

        public ExerciseLogic(IRepository<Exercise> repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public void Create(ExerciseDto entity)
        {
            _repo.Create(_mapper.Map<Exercise>(entity));
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public Exercise Read(int id)
        {
            return _repo.Read(id);
        }

        public IQueryable<Exercise> ReadAll()
        {
            return _repo.ReadAll();
        }

        public void Update(ExerciseDto entity, int id)
        {
            var ent = _mapper.Map<Exercise>(entity);
            ent.ExerciseId = id;
            _repo.Update(ent);
        }


    }
}
