using Data;
using Dtos.Message;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repository
{
	public class MessageRepository : IMessageRepository
	{
		private readonly ApplicationDBContext _context;
		private readonly ILogger<IMessageRepository> _logger;

		public MessageRepository(ApplicationDBContext context, ILogger<IMessageRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<ICollection<MessageDto>> GetAllMessagesBetweenUsers(string userId1, string userId2)
		{
			return await _context.Messages
				.Where(m => m.SenderId == userId1 && m.ReceiverId == userId2 || m.SenderId == userId2 && m.ReceiverId == userId1)
				.Select(msg => new MessageDto
				{
					Id = msg.Id,
					SenderId = msg.SenderId,
					Content = msg.Content,
					CreatedAt = msg.CreatedAt
				})
				.OrderBy(msg => msg.CreatedAt)
				.ToListAsync();
		}

		public async Task<ICollection<LastUserWithMessagesDto>> GetLastUsersWithMessagesBetween(string userId)
		{
			return await _context.Messages
				.Where(m => m.SenderId == userId || m.ReceiverId == userId)
				.GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
				.Select(g => new LastUserWithMessagesDto
				{
					UserId = g.Key,
					Username = _context.Users.FirstOrDefault(u => u.Id == g.Key)!.UserName ?? string.Empty,
					LastMessage = g.OrderByDescending(m => m.CreatedAt).FirstOrDefault()!.Content ?? string.Empty,
					LastMessageDate = g.Max(m => m.CreatedAt)
				})
				.OrderByDescending(u => u.LastMessageDate)
				.ToListAsync();
		}

		public async Task<Message> SendMessage(Message message)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.Messages.AddAsync(message);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return message;
				}
				catch (Exception e)
				{
					await transaction.RollbackAsync();
					_logger.LogError(e, "Failed to send message");
					return null!;
				}
			}
		}
	}
}
