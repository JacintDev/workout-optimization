using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WorkoutOptimization.Models
{
    public class GyroscopeData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GyroscopeDataId { get; set; }
        [Required]
        public float AccelX { get; set; }
        [Required]
        public float AccelY { get; set; }
        [Required]
        public float AccelZ { get; set; }
        [Required]
        public float GyrosX { get; set; }
        [Required]
        public float GyrosY { get; set; }
        [Required]
        public float GyrosZ { get; set; }


        [ForeignKey(nameof(Training))]
        public int? TrainingId { get; set; }
        [JsonIgnore]

        public virtual Training Training { get; set; }

        public DateTime? Date { get; set; }
    }
}
