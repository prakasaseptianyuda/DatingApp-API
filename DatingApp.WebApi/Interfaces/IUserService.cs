using DatingApp.WebApi.Dtos.User;
using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Interfaces
{
    public interface IUserService
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUserAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);

        Task<MemberDto> GetMemberByUsernameAsync(string username);
        Task<MemberDto> GetMemberByIdAsync(int username);
        Task<MemberDto> GetMemberAsync(string username);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<bool> UpdateAsync(AppUser user,MemberUpdateDto memberUpdateDto);
    }
}
