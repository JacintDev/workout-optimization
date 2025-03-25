using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models
{
    public class PromotionDto
    {

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public string? Image { get; set; }
    }
}
