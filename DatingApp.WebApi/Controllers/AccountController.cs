using DatingApp.WebApi.Dtos.User;
using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDto userDto)
        {
            if (await IsUserExist(userDto.Username))
            {
                return BadRequest("Username telah terdaftar");
            }

            using var hmac = new HMACSHA512();
            var user = new User { 
                Username = userDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password)),
                PasswordSalt = hmac.Key,
                CreatedDate = DateTime.Now,
                CreatedBy = "System",
                UpdatedDate = DateTime.Now,
                UpdatedBy = "System"
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserDto
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto userDto)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Username == userDto.Username);
            if (user == null)
            {
                return Unauthorized("Username tidak terdaftar");
            }

            using var hmcac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmcac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Password salah");
                }
            }
            return Ok(new UserDto
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
            });
        }

        private async Task<bool> IsUserExist(string username)
        {
            return await _context.User.AnyAsync(x => x.Username == username);
        }

    }
}
