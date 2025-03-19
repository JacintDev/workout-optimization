using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Models;

namespace WorkoutOptimization.Logic
{
    public class AuthorizationLogic : IAuthorizationLogic
    {
        UserManager<User> _userManager;
        IMapper _mapper;
        public AuthorizationLogic(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<bool> Register(RegisterModel model)
        {
            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                throw new Exception("User with this email already exists");
            }
            
            try
            {
                var result = await _userManager.CreateAsync(_mapper.Map<User>(model), model.Password);
                if (_userManager.Users.Count() == 1)
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync(model.Email), "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync(model.Email), "User");
                }
                return result.Succeeded;
            }
            catch (Exception)
            {

                throw new Exception("User creation failed.");
            }
           
        }
    }
}
