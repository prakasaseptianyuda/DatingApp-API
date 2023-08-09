using AutoMapper;
using DatingApp.WebApi.Dtos.User;
using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,IMapper mapper)
        {
            _userManager = userManager;
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

            var user = _mapper.Map<AppUser>(userDto);

            using var hmac = new HMACSHA512();

            user.CreatedDate = DateTime.Now;
            user.CreatedBy = "System";
            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = "System";

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user,"Member");

            if (!roleResult.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto userDto)
        {
            var user = await _userManager.Users.Include(q => q.Photos).FirstOrDefaultAsync(x => x.UserName == userDto.Username);
            if (user == null)
            {
                return Unauthorized("Username tidak terdaftar");
            }

            var result = await _userManager.CheckPasswordAsync(user, userDto.Password);

            if (!result)
            {
                return Unauthorized("Invalid Password");
            }

            return Ok(new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            });
        }

        private async Task<bool> IsUserExist(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username);
        }

    }
}
