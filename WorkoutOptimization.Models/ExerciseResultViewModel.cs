using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models
{
    public class ExerciseResultViewModel
    {
        public int ExerciseId { get; set; }

        public int TrainingId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
