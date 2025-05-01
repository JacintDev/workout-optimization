using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public DateTime? End { get; set; }
        [Required]
        public bool isActive { get; set; }

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
            this.GyroscopeData=new HashSet<GyroscopeData>();
        }

    }
}
