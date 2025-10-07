using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models
{
    public class DailyWeightViewModel
    {
        public int DailyWeightId { get; set; }
        public DateTime Date { get; set; } 
        [Required]
        public float Weight { get; set; }
    }
}
