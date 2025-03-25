using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        
        [Length(2,20)]
        public string? FirstName { get; set; }
        
        [Length(2, 20)]
        public string? LastName { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        [NotMapped]
        public int GetAge 
        {
            get => DateOfBirth.HasValue ? DateTime.Now.Year - DateOfBirth.Value.Year : 0;
        }
        public int? Weight { get; set; }
        public int? Height { get; set; }

        public Level? Level { get; set; }
        public Sex? Sex { get; set; }
        public string? MacAddress { get; set; }
        [JsonIgnore]
        
        public virtual ICollection<Exercise> Exercises { get; set; }
        [JsonIgnore]
        public virtual ICollection<GyroscopeData> GyroscopeData { get; set; }
        public User()
        {
            this.Exercises=new HashSet<Exercise>();
            this.GyroscopeData = new HashSet<GyroscopeData>();
        }





    }
}
