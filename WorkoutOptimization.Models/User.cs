using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public enum Level
    {
        Beginner,
        Intermediate,
        Advanced
    }
    public enum Sex
    {
        Man,
        Woman
    }

    
namespace WorkoutOptimization.Models
{
        
    public class User : IdentityUser
    {
        [Required]
        [Length(2,20)]
        public string FirstName { get; set; }
        [Required]
        [Length(2, 20)]
        public string LastName { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [NotMapped]
        public int GetAge 
        {
            get => DateOfBirth.HasValue ? DateTime.Now.Year - DateOfBirth.Value.Year : 0;
        }
        public int Weight { get; set; }
        public int Height { get; set; }

        public Level Level { get; set; }
        public Sex Sex { get; set; }



    }
}
