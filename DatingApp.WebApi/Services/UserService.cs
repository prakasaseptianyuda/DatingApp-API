using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.WebApi.Dtos.User;
using DatingApp.WebApi.Entities;
using DatingApp.WebApi.Helpers;
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
            var member = await _context.Users.Where(x => x.UserName == username).
                ProjectTo<MemberDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return member;
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge -1);

            var query = _context.Users.AsQueryable();
            query = query.Where(x => x.UserName != userParams.CurrentUsername && x.Gender == userParams.Gender && x.Birthdate >= minDob && x.Birthdate <= maxDob);
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.CreatedDate),
                _ => query.OrderByDescending(u => u.LastActive)
            };


            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<MemberDto> GetMemberByUsernameAsync(string username)
        {
            return await _context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Username.ToLower() == username);
        }

        public async Task<MemberDto> GetMemberByIdAsync(int id)
        {
            return await _context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<IEnumerable<AppUser>> GetUserAsync()
        {
            return await _context.Users.Include(x => x.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<bool> UpdateAsync(AppUser user,MemberUpdateDto memberUpdateDto)
        {
            _mapper.Map(memberUpdateDto,user);

            Update(user);

            return await SaveAllAsync();
        }
    }
}
