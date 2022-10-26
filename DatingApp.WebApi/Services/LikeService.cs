using DatingApp.WebApi.Dtos.Like;
using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Extensions;
using DatingApp.WebApi.Helpers;
using DatingApp.WebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Services
{
    public class LikeService : ILikeService
    {
        private readonly DataContext _context;

        public LikeService(DataContext context)
        {
            
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {
            return await _context.Like.FindAsync(sourceUserId,likeUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = _context.User.OrderBy(x => x.Username).AsQueryable();
            var likes = _context.Like.AsQueryable();

            if (likeParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if (likeParams.Predicate == "likeBy")
            {
                likes = likes.Where(like => like.LikedUserId == likeParams.UserId) ;
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                Username = user.Username,
                KnownAs = user.KnownAs,
                Age = user.Birthdate.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.UserId
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<User> GetUserWithLikes(int userId)
        {
            return await _context.User.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
