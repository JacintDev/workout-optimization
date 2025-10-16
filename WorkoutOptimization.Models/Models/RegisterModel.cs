using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WorkoutOptimization.Models.Models
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Length(2, 20)]
        public string FirstName { get; set; }
        [Required]
        [Length(2, 20)]
        public string LastName { get; set; }
        public Sex Sex { get; set; }

    }
}
