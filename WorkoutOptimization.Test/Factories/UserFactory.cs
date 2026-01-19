using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Test.Factories
{
    public static class UserFactory
    {

        private static Faker<RegisterModel> _userFaker = new Faker<RegisterModel>("en")
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.Password, f => f.Internet.Password(8))
            .RuleFor(u => u.Sex, f => f.PickRandom<Sex>());

        public static RegisterModel CreateValidUser() {
        
            return _userFaker.Generate();
        }
    }
}
