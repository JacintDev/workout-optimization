using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models
{
    public class Training
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrainingId { get; set; }
        [Required]
        public DateTime Start {  get; set; }
        [Required]
        public DateTime End { get; set; }

        [ForeignKey(nameof(Exercise))]
        public int ExerciseId { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public virtual Exercise Exercise { get; set; }
        public virtual User User { get; set; }

    }
}
