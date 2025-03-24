using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models
{
    public class TrainingDto
    {
      
      
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }

        [ForeignKey(nameof(Exercise))]
        public int ExerciseId { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

    }
}
