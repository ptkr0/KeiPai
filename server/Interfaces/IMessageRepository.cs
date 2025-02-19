using Dtos.Message;
using Models;

namespace Interfaces
{
	public interface IMessageRepository
	{
		Task<ICollection<MessageDto>> GetAllMessagesBetweenUsers(string userId1, string userId2);
		Task<ICollection<LastUserWithMessagesDto>> GetLastUsersWithMessagesBetween(string userId);
		Task<Message> SendMessage(Message message);
	}
}
