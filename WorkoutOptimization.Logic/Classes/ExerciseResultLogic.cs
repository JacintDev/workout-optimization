using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<ExerciseResultReturnedValueModel>> ReadAllById(int trainingid)
        {
            var res = await _repo.ReadAll()
                .Where(x => x.TrainingId == trainingid)
                .ToListAsync();
            var groupby = res
                .GroupBy(x => x.Training.Start.Date)
                .Select(x => new ExerciseResultReturnedValueModel()
            {
                Date = x.Key.ToShortDateString(),
                Correct = x.Count(y => y.IsCorrect),
                InCorrect= x.Count(y=> !y.IsCorrect)
            });
            return groupby;


        } 

        public async Task<IEnumerable<ExerciseResultReturnedValueModel>> ReadAllByUser(User user)
        {
            var res = await _repo.ReadAll()
                .Where(x => x.Training.UserId == user.Id)
                .ToListAsync();
            var groupby = res
                .GroupBy(x => x.Training.Start.Date)
                .Select(x => new ExerciseResultReturnedValueModel()
                {
                    Date = x.Key.ToShortDateString(),
                    Correct = x.Count(y => y.IsCorrect),
                    InCorrect = x.Count(y => !y.IsCorrect)
                });
            return groupby;
        }
    }
}
