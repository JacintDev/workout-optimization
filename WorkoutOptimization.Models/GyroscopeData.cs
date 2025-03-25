using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public virtual User User { get; set; }


        [ForeignKey(nameof(Training))]
        public int TrainingId { get; set; }

        public virtual Training Training { get; set; }

        public DateTime? Date { get; set; }
    }
}
