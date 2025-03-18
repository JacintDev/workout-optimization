using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutOptimization.Models
{
    public class GyroscopeDataDto
    {
        
        public float AccelX { get; set; }
        
        public float AccelY { get; set; }
        
        public float AccelZ { get; set; }
        
        public float GyrosX { get; set; }
        
        public float GyrosY { get; set; }
        
        public float GyrosZ { get; set; }
        public DateTime? Date { get; set; }
    }
}
