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
    public class TrainingLogic : ITrainingLogic
    {
        readonly IMapper _mapper;
        readonly IRepository<Training> _repo;

        public TrainingLogic(IRepository<Training> repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<bool> Create(TrainingDto entity, User user)
        {
            var training= _mapper.Map<Training>(entity);
            training.User = user;
            training.UserId=user.Id;
            try
            {
                _repo.Create(training);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public (bool, Training?) GetIsActiveTraining(User user)
        {
            var training= _repo.ReadAll().FirstOrDefault(x => x.UserId == user.Id && x.isActive == true);
            return (training !=null, training);
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

        public void StopTraining(int id, User user)
        {
            var training = _repo.Read(id);
            if (training == null)
            {
                throw new InvalidOperationException("Training not found with: " + id);
            }
            training.isActive = false;
            training.User = user;
            training.UserId = user.Id;
            training.End = DateTime.Now;
            _repo.Update(training);
        }

        public void Update(TrainingDto entity, int id, User user)
        {
            var ent = _mapper.Map<Training>(entity);
            ent.User = user;
            ent.UserId = user.Id;
            ent.TrainingId = id;
            _repo.Update(ent);
        }

        public int CountTrainings(User user)
        {
            var result=_repo.ReadAll().Count(x=>x.UserId == user.Id);
            return result;
        }

        public IQueryable<CountWorkoutSessionModel> CountWorkoutSessions(User user)
        {
            var res = _repo.ReadAll()
                .Where(x=> x.UserId == user.Id)
                .GroupBy(x => x.Start.Date)
                .Select(g => new CountWorkoutSessionModel
                {
                    Date = g.Key.ToShortDateString(),
                    Count = g.Count()
                });
            return res;
        }
    }
}
