using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models.Models
{
    public class PulseSendModel
    {
        [Required]
        public double Pulse { get; set; }
    }
}
