using AutoMapper;
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
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService,IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        /// <summary>
        /// test test test
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDto userDto)
        {
            if (await IsUserExist(userDto.Username))
            {
                return BadRequest("Username telah terdaftar");
            }

            var user = _mapper.Map<User>(userDto);

            using var hmac = new HMACSHA512();

            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));
            user.PasswordSalt = hmac.Key;
            user.CreatedDate = DateTime.Now;
            user.CreatedBy = "System";
            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = "System";


            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new UserDto
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto userDto)
        {
            var user = await _context.User.Include(q => q.Photos).FirstOrDefaultAsync(x => x.Username == userDto.Username);
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
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            });
        }

        private async Task<bool> IsUserExist(string username)
        {
            return await _context.User.AnyAsync(x => x.Username == username);
        }

    }
}
