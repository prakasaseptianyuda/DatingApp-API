using DatingApp.WebApi.Dtos.Like;
using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Interfaces
{
    public interface ILikeService
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likeUserId);
        Task<User> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);
    }
}
