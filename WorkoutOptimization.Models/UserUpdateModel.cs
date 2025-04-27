using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models
{
    public class UserUpdateModel
    {
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public int? Weight { get; set; }
        [Required]
        public int? Height { get; set; }
        [Required]
        public Level? Level { get; set; }
    }
}
