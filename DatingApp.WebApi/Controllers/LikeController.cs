using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Extensions;
using DatingApp.WebApi.Helpers;
using DatingApp.WebApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ILikeService _likeService;

        public LikeController(IUserService userService, ILikeService likeService)
        {
            _userService = userService;
            _likeService = likeService;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userService.GetMemberByUsernameAsync(username);
            var sourceUser = await _likeService.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
            {
                return NotFound();
            }

            if (sourceUser.Username == username)
            {
                return BadRequest("You cannot like yourself.");
            }

            var userLike = await _likeService.GetUserLike(sourceUserId, likedUser.UserId);

            if (userLike != null)
            {
                return BadRequest("You Already like this user.");
            }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.UserId
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _userService.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to like user.");
        }

        [HttpGet]
        public async Task<ActionResult> GetUserLikes([FromQuery]LikeParams likeParams)
        {
            likeParams.UserId = User.GetUserId();
            var users = await _likeService.GetUserLikes(likeParams);

            Response.AddPagionationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPage);

            return Ok(users);
        }
    }
}
