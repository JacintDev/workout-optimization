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
        }

    }
}
