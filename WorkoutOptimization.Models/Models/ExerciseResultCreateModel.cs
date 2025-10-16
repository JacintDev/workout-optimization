using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models.Models
{
    public class ExerciseResultCreateModel
    {
        [Required]
        public int TrainingId { get; set; }
        [Required]
        public bool IsCorrect { get; set; }
    }
}
