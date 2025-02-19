using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutOptimization.Models
{
    public class GyrosscopeData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GyroscopeDataId { get; set; }
        [Required]
        public int AccelX { get; set; }
        [Required]
        public int AccelY { get; set; }
        [Required]
        public int AccelZ { get; set; }
        [Required]
        public int GyrosX { get; set; }
        [Required]
        public int GyrosY { get; set; }
        [Required]
        public int GyrosZ { get; set; }
        public DateTime? Date { get; set; }
    }
}
