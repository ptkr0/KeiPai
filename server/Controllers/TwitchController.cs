using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;
using Swashbuckle.AspNetCore.Annotations;
using Interfaces;
using Dtos.Twitch;
using Extensions;

namespace Controllers
{
	[Route("api/twitch")]
	[ApiController]
	public class TwitchController : ControllerBase
	{
		private readonly ITwitchService _twitchService;
		private readonly ITwitchRepository _twitchRepo;

		public TwitchController(ITwitchService twitchService, ITwitchRepository twitchRepo)
		{
			_twitchService = twitchService;
			_twitchRepo = twitchRepo;
		}

		[Authorize(Roles = "Influencer")]
		[HttpPost("connect")]
		[SwaggerOperation(Summary = "uses a string from Twitch OAuth 2.0 screen to connect twitch account")]
		public async Task<IActionResult> ConnectTwitch([FromBody] string code)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var twitchAccount = await _twitchRepo.GetTwitchAccount(userId);
			if (twitchAccount != null) return BadRequest("Twitch account already connected");

			var response = await _twitchService.ExchangeCodeForTokens(code);
			if (response == null) return BadRequest("Failed to exchange code for tokens");

			var channelInfo = await _twitchService.GetChannelInfo(response.AccessToken);
			if (channelInfo == null) return BadRequest("Failed to get channel info");

			var webhooks = await _twitchService.SubscribeToWebhooks(channelInfo.ChannelId);
			if (webhooks == false) return BadRequest("Failed to subscribe to webhooks");

			twitchAccount = new TwitchAccount
			{
				UserId = userId,
				RefreshToken = response.RefreshToken,
				TwitchChannelId = channelInfo.ChannelId,
				ChannelName = channelInfo.ChannelName,
				FollowerCount = channelInfo.FollowerCount,
				LastCrawlDate = DateTime.Now,
				IsAffiliateOrPartner = channelInfo.IsAffiliateOrPartner,
			};

			twitchAccount = await _twitchRepo.AddTwitchAccount(twitchAccount);
			if (twitchAccount == null) return BadRequest("Failed to add twitch account");

			return Ok(twitchAccount);
		}

		[Authorize(Roles = "Influencer")]
		[HttpDelete("disconnect")]
		[SwaggerOperation(Summary = "disconnects twitch account")]
		public async Task<IActionResult> DisconnectTwitch()
		{
			var userId = User.GetUserId();
			if (userId == null) return Unauthorized();

			var twitchAccount = await _twitchRepo.GetTwitchAccount(userId);
			if (twitchAccount == null) return BadRequest("Twitch account not connected");

			var webhooks = await _twitchService.UnsubscribeFromWebhooks(twitchAccount.TwitchChannelId);
			if (webhooks == false) return BadRequest("Failed to unsubscribe from webhooks");

			var result = await _twitchRepo.DeleteTwitchAccount(twitchAccount);
			if (result == null) return BadRequest("Failed to disconnect twitch account");

			return Ok(result);
		}

		[HttpGet("streams/{userId}")]
		[Authorize]
		[SwaggerOperation(Summary = "gets all livestreams of a user")]
		public async Task<IActionResult> GetStreams(string userId, int pageNumber = 1)
		{
			int pageSize = 20;

			var streams = await _twitchRepo.GetAllStreams(userId, pageSize, pageNumber);

			return Ok(streams);
		}

		[HttpGet("stream/{id}")]
		[Authorize]
		[SwaggerOperation(Summary = "gets a stream by id")]
		public async Task<IActionResult> GetStream(int id)
		{
			var stream = await _twitchRepo.GetStreamById(id);
			if (stream == null) return NotFound();
			return Ok(stream);
		}

		[HttpPost("eventsub")]
		[SwaggerOperation(Summary = "twitch EventSub callback")]
		public async Task<IActionResult> Callback()
		{
			var messageType = Request.Headers["Twitch-Eventsub-Message-Type"].FirstOrDefault();

			using var reader = new StreamReader(Request.Body);
			var body = await reader.ReadToEndAsync();

			switch (messageType)
			{
				case "webhook_callback_verification":

					var json = JsonDocument.Parse(body);
					var challenge = json.RootElement.GetProperty("challenge").GetString();

					return Content(challenge!, "text/plain");

				case "notification":

					var notificationJson = JsonDocument.Parse(body);
					var eventType = notificationJson.RootElement.GetProperty("subscription").GetProperty("type").GetString();
					var userId = notificationJson.RootElement.GetProperty("event").GetProperty("broadcaster_user_id").GetString();

					if (userId == null) return BadRequest("Missing broadcaster_user_id");

					var twitchAccount = await _twitchRepo.GetTwitchAccountByChannelId(userId);
					if (twitchAccount == null) return BadRequest("Invalid user");

					switch (eventType)
					{
						case "stream.online":

							var streamOnline = JsonSerializer.Deserialize<StreamOnlineNotifcation>(notificationJson.RootElement.GetProperty("event").GetRawText());
							if (streamOnline == null) return BadRequest("Failed to parse stream online notification");

							Console.WriteLine("Stream online", streamOnline.broadcaster_user_name, streamOnline.broadcaster_user_id, streamOnline.id);

							var livestream = new Content()
							{
								Type = "twitch",
								UserId = twitchAccount.UserId,
								TwitchStream = new TwitchStream()
								{
									StreamId = streamOnline.id,
									StartDate = DateTime.Now,
								}
							};

							var response = await _twitchRepo.StartStream(livestream);
							if (response == null) return BadRequest("Failed to start stream");

							break;

						case "stream.offline":

							var streamOffline = JsonSerializer.Deserialize<StreamOfflineNotification>(notificationJson.RootElement.GetProperty("event").GetRawText());
							if (streamOffline == null) return BadRequest("Failed to parse stream offline notification");

							Console.WriteLine("Stream online", streamOffline.broadcaster_user_name, streamOffline.broadcaster_user_id);

							var activeStream = await _twitchRepo.GetActiveStreamByUsedId(twitchAccount.UserId);
							var accessToken = await _twitchService.GetAppAccessToken();
							var additionalStreamInfo = await _twitchService.GetAdditionalStreamInfo(accessToken, twitchAccount.TwitchChannelId);
							if (additionalStreamInfo != null && activeStream != null && additionalStreamInfo.StreamId == activeStream.TwitchStream!.StreamId)
							{
								activeStream.TwitchStream.Url = additionalStreamInfo.Url;
								activeStream.TwitchStream.Thumbnail = additionalStreamInfo.ThumbnailUrl;
								await _twitchRepo.StopStream(activeStream, twitchAccount.UserId);
							}
							else if (activeStream != null)
							{
								await _twitchRepo.StopStream(activeStream, twitchAccount.UserId);
							}

							break;

					}
					return Ok("Gladgers");

				case "revocation":

					Console.WriteLine("Subscription revoked");
					// todo: if subscription revoked, delete the refresh token from the database
					return Ok();

				default:
					return Ok();
			}
		}
	}
}
