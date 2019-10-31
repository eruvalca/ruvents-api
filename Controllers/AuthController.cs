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
            userDTO.Username = userDTO.Username.ToLower();

            if (await _context.Users.AnyAsync(x => x.Username == userDTO.Username))
            {
                return BadRequest("Username already exists.");
            }

            CreatePasswordHash(userDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User userToCreate = new User
            {
                Username = userDTO.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
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
            User matchingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == userDTO.Username.ToLower());

            if (matchingUser == null)
            {
                return NoContent();
            }
            else if (!VerifyPasswordHash(userDTO.Password, matchingUser.PasswordHash, matchingUser.PasswordSalt))
            {
                return Unauthorized();
            }

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, matchingUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, matchingUser.Username)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
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
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == User.Identity.Name);

            if (currentUser == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(new UserViewModel()
                {
                    UserId = currentUser.UserId,
                    Username = currentUser.Username,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    NickName = currentUser.NickName
                });
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}