using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.WebApi.Dtos.User;
using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserService(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            var member = await _context.User.Where(x => x.Username == username).
                ProjectTo<MemberDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return member;
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            var members = await _context.User.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToListAsync();
            return members;
        }

        public async Task<MemberDto> GetMemberByUsernameAsync(string username)
        {
            return await _context.User.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<IEnumerable<User>> GetUserAsync()
        {
            return await _context.User.Include(x => x.Photos).ToListAsync();
        }

        public async Task<MemberDto> GetUserByIdAsync(int id)
        {
            return await _context.User.Where(x => x.UserId == id).
                ProjectTo<MemberDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.User.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<bool> UpdateAsync(User user,MemberUpdateDto memberUpdateDto)
        {
            _mapper.Map(memberUpdateDto,user);

            Update(user);

            return await SaveAllAsync();
        }
    }
}
