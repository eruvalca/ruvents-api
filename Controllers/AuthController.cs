using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ruvents_api.Models;
using ruvents_api.ViewModels;

namespace ruvents_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly RuventsContext _context;

        public AuthController(IConfiguration config, RuventsContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            if (await _context.Users.AnyAsync(x => x.PhoneNumber == userDTO.PhoneNumber))
            {
                return BadRequest("User already exists.");
            }

            User userToCreate = new User
            {
                PhoneNumber = userDTO.PhoneNumber,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                NickName = userDTO.NickName
            };

            _context.Add(userToCreate);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            User matchingUser = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == userDTO.PhoneNumber);

            if (matchingUser == null)
            {
                return NoContent();
            }

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, matchingUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, matchingUser.PhoneNumber)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == User.Identity.Name);

            if (currentUser == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(new UserViewModel()
                {
                    UserId = currentUser.UserId,
                    PhoneNumber = currentUser.PhoneNumber,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    NickName = currentUser.NickName
                });
            }
        }
    }
}