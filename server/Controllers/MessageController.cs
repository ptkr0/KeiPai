using Dtos.Message;
using Extensions;
using Interfaces;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Controllers
{
	[Route("api/message")]
	[ApiController]
	public class MessageController : ControllerBase
	{
		private readonly IMessageRepository _messageRepository;
		private readonly IUserService _userService;

		public MessageController(IMessageRepository messageRepository, IUserService userService)
		{
			_messageRepository = messageRepository;
			_userService = userService;
		}

		[HttpGet("users")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get last users with messages between (user and last message was sent between them)")]
		public async Task<IActionResult> GetLastUsersWithMessagesBetween()
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var lastUsersWithMessages = await _messageRepository.GetLastUsersWithMessagesBetween(userId);

			return Ok(lastUsersWithMessages);
		}

		[HttpGet("{userId}")]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to get all messages between 2 users")]
		public async Task<IActionResult> GetAllMessagesBetweenUsers([FromRoute] string userId, [FromQuery] int? pageNumber = 1)
		{
			var currentUserId = User.GetUserId();
			if (currentUserId == null) return Unauthorized();

			var userInfo = await _userService.GetBasicUserInfo(userId);

			var messages = await _messageRepository.GetAllMessagesBetweenUsers(currentUserId, userId);

			var response = new
			{
				user = userInfo,
				messages
			};

			return Ok(response);
		}

		[HttpPost]
		[Authorize]
		[SwaggerOperation(Summary = "endpoint to send a message to a user")]
		public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var message = new Message
			{
				SenderId = userId,
				ReceiverId = sendMessageDto.ReceiverId,
				Content = sendMessageDto.Content,
				CreatedAt = DateTime.Now
			};

			var sentMessage = await _messageRepository.SendMessage(message);

			return Ok(sentMessage);
		}
	}
}
