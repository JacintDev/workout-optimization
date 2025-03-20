using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models
{
    public class ExerciseDto
    {
        //copy properties from Exercise.cs
        public string Name { get; set; }
        public string Description { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public string? Video { get; set; }
    }
}
