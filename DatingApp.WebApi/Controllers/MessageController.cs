using AutoMapper;
using DatingApp.WebApi.Dtos.Message;
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
    public class MessageController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;

        public MessageController(IUserService userService, IMessageService messageService, IMapper mapper)
        {
            _userService = userService;
            _messageService = messageService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send message to yourself");
            }

            var sender = await _userService.GetUserByUsernameAsync(username);
            var recipient = await _userService.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
            {
                return NotFound();
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageService.AddMessage(message);

            if (await _messageService.SaveAllAsync())
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageService.GetMessageForUser(messageParams);

            Response.AddPagionationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPage);

            return Ok(messages);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult> GetMessagerThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _messageService.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _messageService.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
            {
                return Unauthorized();
            }

            if (message.Sender.UserName == username)
            {
                message.SenderDeleted = true;
            }

            if (message.Recipient.UserName == username)
            {
                message.RecipientDeleted = true;
            }

            if (message.RecipientDeleted && message.SenderDeleted)
            {
                _messageService.DeleteMessage(message);
            }

            if (await _messageService.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Problem deleting message");
        }
    }
}
