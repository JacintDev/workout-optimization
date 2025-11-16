using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Models.Entities
{
    public class MlModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MlModelId { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Version { get; set; }

        [Required]
        public byte[] ModelData { get; set; }
        [Required]
        public string DataMin { get; set; }
        [Required]
        public string DataRange { get; set; }
        public float Accurate { get; set; }




    }
}
