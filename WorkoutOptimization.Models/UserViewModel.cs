using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WorkoutOptimization.Models
{
    public class UserViewModel
    {
        [JsonPropertyName("UserId")]
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int GetAge
        {
            get => DateOfBirth.HasValue ? DateTime.Now.Year - DateOfBirth.Value.Year : 0;
        }
        public int? Weight { get; set; }
        public int? Height { get; set; }

        public Level? Level { get; set; }
        public Sex? Sex { get; set; }
        public string? MacAddress { get; set; }
    }
}
