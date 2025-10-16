using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models.Entities
{
    public class Training
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrainingId { get; set; }
        [Required]
        public DateTime Start {  get; set; }
        public DateTime? End { get; set; }
        [Required]
        public bool isActive { get; set; }
        public bool? IsCorrect { get; set; }

        public virtual ICollection<ExerciseResult> ExerciseResults { get; set; }

        [ForeignKey(nameof(Exercise))]
        public int ExerciseId { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [JsonIgnore]
        public virtual Exercise Exercise { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<GyroscopeData> GyroscopeData { get; set; }

        public Training()
        {
            GyroscopeData=new HashSet<GyroscopeData>();
            ExerciseResults = new HashSet<ExerciseResult>();
        }

    }
}
