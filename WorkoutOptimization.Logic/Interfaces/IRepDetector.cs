using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Logic.Interfaces
{
    public interface IRepDetector
    {
        IEnumerable<IReadOnlyList<GyroscopeDataDto>> AddSample(GyroscopeDataDto sample);
    }
}
