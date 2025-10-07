using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic.Helpers
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<GyroscopeData, GyroscopeDataDto>().ReverseMap();
            CreateMap<User,RegisterModel>().ReverseMap();
            CreateMap<User, LoginModel>().ReverseMap();
            CreateMap<Exercise, ExerciseDto>().ReverseMap();
            CreateMap<Training, TrainingDto>().ReverseMap();
            CreateMap<Promotion, PromotionDto>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<User, UserUpdateModel>().ReverseMap();
            CreateMap<DailyWeight, DailyWeightCreateModel>().ReverseMap();
            CreateMap<DailyWeight, DailyWeightViewModel>();
            //TODO: refactor these automapper reversemap
        }

    }
}
