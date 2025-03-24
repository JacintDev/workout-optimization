using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum MuscleGroup
{
    Chest,
    Back,
    Legs,
    Shoulders,
    Arms,
    Abs
}
namespace WorkoutOptimization.Models
{
    public class Exercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExerciseId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public MuscleGroup MuscleGroup { get; set; }
        public string? Video { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Exercise()
        {
            Users=new HashSet<User>();
        }
    }
}
