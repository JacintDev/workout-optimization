using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models
{
    public class DailyWeight
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DailyWeightId { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        public float Weight { get; set; }

        public virtual User User { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

    }
}
