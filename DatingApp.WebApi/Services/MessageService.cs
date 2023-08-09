using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.WebApi.Dtos.Message;
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
    public class MessageService : IMessageService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Message.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Message.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Message
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Message
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.DateRead == null && u.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Message
                .Include(q => q.Sender).ThenInclude(q => q.Photos)
                .Include(q => q.Recipient).ThenInclude(q => q.Photos)
                .Where(x => x.RecipientUsername == currentUsername && x.RecipientDeleted == false && x.SenderUsername == recipientUsername ||
            x.RecipientUsername == recipientUsername && x.SenderUsername == currentUsername && x.SenderDeleted == false).OrderByDescending(o => o.MessageSent).ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
