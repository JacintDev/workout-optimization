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
    public class ExerciseResultLogic : IExerciseResultLogic
    {

        readonly IRepository<ExerciseResult> _repo;
        readonly IMapper _mapper;

        public ExerciseResultLogic(IRepository<ExerciseResult> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task CreateExerciseResult(ExerciseResultCreateModel model)
        {
            var ent = _mapper.Map<ExerciseResult>(model);
            await _repo.CreateAsync(ent);
        }
    }
}
