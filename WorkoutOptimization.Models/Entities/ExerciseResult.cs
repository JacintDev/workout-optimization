using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models.Entities
{
    public class ExerciseResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExerciseId { get; set; }

        [ForeignKey(nameof(Training))]
        public int TrainingId { get; set; }
        public virtual Training Training { get; set; }
        public bool IsCorrect { get; set; }


    }
}
