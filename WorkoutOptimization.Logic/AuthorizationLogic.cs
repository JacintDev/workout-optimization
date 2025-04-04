using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
                var isFirstUser = !(await _userManager.Users.AnyAsync());
                var user = _mapper.Map<User>(model);
                user.UserName=user.Email;

                var result = await _userManager.CreateAsync(user, model.Password);

                var role = isFirstUser ? "Admin" : "User";
                await _userManager.AddToRoleAsync(user, role);
                return result.Succeeded;
            }
            catch (Exception)
            {

                throw new Exception("User creation failed.");
            }
           
        }

        public async Task<(bool isAuthenticated, string Token, DateTime Expiration)> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var claim = new List<Claim> 
                { new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                  new Claim(ClaimTypes.Name, user.Email)};
                foreach (var role in await _userManager.GetRolesAsync(user))
                {
                    claim.Add(new Claim(ClaimTypes.Role, role));
                }
                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("nagyonhosszutitkoskodhelyenagyonhosszutitkoskodhelye"));
                var token = new JwtSecurityToken(
                    issuer: "http://www.security.org", audience: "http://www.security.org",
                    claims: claim, expires: DateTime.Now.AddMinutes(180),
                    signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );
               
                return (true, new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
            }
            return (false, null, DateTime.MinValue);
        }
    }
}
