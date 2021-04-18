using DatingApp.WebApi.Dtos.User;
using DatingApp.WebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Interfaces
{
    public interface IUserService
    {
        void Update(User user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<User>> GetUserAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByNameAsync(string username);
        Task<MemberDto> GetMemberAsync(string username);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
    }
}
