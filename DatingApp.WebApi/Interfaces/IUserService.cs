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
        Task<MemberDto> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);

        Task<MemberDto> GetMemberByUsernameAsync(string username);
        Task<MemberDto> GetMemberAsync(string username);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<bool> UpdateAsync(User user,MemberUpdateDto memberUpdateDto);
    }
}
