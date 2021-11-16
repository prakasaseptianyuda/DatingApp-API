using AutoMapper;
using DatingApp.WebApi.Dtos.User;
using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Extensions;
using DatingApp.WebApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserController(IUserService userService, IMapper mapper, IPhotoService photoService)
        {
            _userService = userService;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult> GetMembers()
        {
            var users = await _userService.GetMembersAsync();
            //var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(users);
        }

        [HttpGet("{id}",Name ="GetUser")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            //var userToReturn = _mapper.Map<MemberDto>(user);
            return Ok(user);
        }

        [HttpGet("GetMemberByUsername/{username}")]
        public async Task<ActionResult> GetMemberByUsername(string username)
        {
            var user = await _userService.GetMemberByUsernameAsync(username);
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());
            var isUpdated =  await _userService.UpdateAsync(user, memberUpdateDto);
            return Ok();
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult> AddPhoto(IFormFile file)
        {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userService.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser",new { id = user.UserId }, _mapper.Map<PhotoDto>(photo));
                //return Ok(_mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem Adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<IActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.PhotoId == photoId);

            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo.");
            }

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }
            photo.IsMain = true;

            if (await _userService.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to set main photo.");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.PhotoId == photoId);
            if (photo == null)
            {
                return NotFound();
            }
            if (photo.IsMain)
            {
                return BadRequest("Cannot delete main photo");
            }
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }
            user.Photos.Remove(photo);

            if (await _userService.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to delete photo");
        }

    }
}
