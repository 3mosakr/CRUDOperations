﻿using CRUDOperations.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRUDOperations.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(JwtOptions jwtOptions, ApplicationDbContext context) : ControllerBase
    {
        // Method used to Generate Token
        [HttpPost]
        [Route("auth")]
        public ActionResult<string> AuthenticateUser(AuthenticationRequest request)
        {
            // validate username and password from db for test
            var user = context.Set<User>().FirstOrDefault(x=>x.username == request.UserName 
                && x.password == request.Password);
            if (user == null)
                return Unauthorized();
            

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)), 
                    SecurityAlgorithms.HmacSha256
                    ),
                // user informations
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new (ClaimTypes.Name, user.username)
                })
            }; 
            var securtyToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securtyToken);
            
            return Ok(accessToken);
        }
    }
}
