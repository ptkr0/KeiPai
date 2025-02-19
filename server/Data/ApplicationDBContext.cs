using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
	public class ApplicationDBContext : IdentityDbContext<User>
	{
		public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

		public DbSet<Game> Games { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<GameScreenshot> GameScreenshots { get; set; }
		public DbSet<Key> Keys { get; set; }
		public DbSet<Platform> Platforms { get; set; }
		public DbSet<Developer> Developers { get; set; }
		public DbSet<Campaign> Campaigns { get; set; }
		public DbSet<Influencer> Influencers { get; set; }
		public DbSet<CampaignKey> CampaignKeys { get; set; }
		public DbSet<YoutubeAccount> YoutubeAccounts { get; set; }
		public DbSet<TwitchAccount> TwitchAccounts { get; set; }
		public DbSet<Content> Contents { get; set; }
		public DbSet<YoutubeVideo> YoutubeVideos { get; set; }
		public DbSet<TwitchStream> TwitchStreams { get; set; }
		public DbSet<OtherMedia> OtherMedias { get; set; }
		public DbSet<OtherContent> OtherContents { get; set; }
		public DbSet<Request> Requests { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<TwitchStreamSnapshot> TwitchStreamSnapshots { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CampaignKey>()
				.HasKey(ck => new { ck.CampaignId, ck.PlatformId });

			modelBuilder.Entity<CampaignKey>()
				.HasOne(ck => ck.Campaign)
				.WithMany(c => c.Keys)
				.HasForeignKey(ck => ck.CampaignId);

			modelBuilder.Entity<CampaignKey>()
				.HasOne(ck => ck.Platform)
				.WithMany()
				.HasForeignKey(ck => ck.PlatformId);

			modelBuilder.Entity<User>()
				.HasOne(u => u.Developer)
				.WithOne()
				.HasForeignKey<Developer>(d => d.UserId);

			modelBuilder.Entity<User>()
				.HasOne(u => u.Influencer)
				.WithOne()
				.HasForeignKey<Influencer>(i => i.UserId);

			modelBuilder.Entity<Developer>(entity =>
			{
				entity.HasKey(d => d.UserId);

				entity.HasOne(d => d.User)
					.WithOne(u => u.Developer)
					.HasForeignKey<Developer>(d => d.UserId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<Influencer>(entity =>
			{
				entity.HasKey(i => i.UserId);

				entity.HasOne(i => i.User)
					.WithOne(u => u.Influencer)
					.HasForeignKey<Influencer>(i => i.UserId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<Message>()
				.HasOne(m => m.Sender)
				.WithMany(u => u.SentMessages)
				.HasForeignKey(m => m.SenderId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Message>()
				.HasOne(m => m.Receiver)
				.WithMany(u => u.ReceivedMessages)
				.HasForeignKey(m => m.ReceiverId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Game>()
				.HasMany(g => g.Tags)
				.WithMany(t => t.Games);

			modelBuilder.Entity<Influencer>()
				.HasOne(i => i.YoutubeAccount)
				.WithOne()
				.HasForeignKey<YoutubeAccount>(ya => ya.UserId);

			modelBuilder.Entity<Influencer>()
				.HasOne(i => i.TwitchAccount)
				.WithOne()
				.HasForeignKey<TwitchAccount>(ta => ta.UserId);

			modelBuilder.Entity<Influencer>()
				.HasOne(i => i.OtherMedia)
				.WithOne()
				.HasForeignKey<OtherMedia>(ya => ya.UserId);

			modelBuilder.Entity<Influencer>()
				.HasMany(i => i.Contents)
				.WithOne(c => c.Influencer)
				.HasForeignKey(c => c.UserId);

			modelBuilder.Entity<Content>()
				.HasOne(y => y.YoutubeVideo)
				.WithOne()
				.HasForeignKey<YoutubeVideo>(y => y.ContentId);

			modelBuilder.Entity<Content>()
				.HasOne(y => y.TwitchStream)
				.WithOne()
				.HasForeignKey<TwitchStream>(y => y.ContentId);

			modelBuilder.Entity<Content>()
				.HasOne(y => y.OtherContent)
				.WithOne()
				.HasForeignKey<OtherContent>(y => y.ContentId);

			modelBuilder.Entity<Game>()
				.HasMany(g => g.Contents)
				.WithMany(c => c.Games);

			modelBuilder.Entity<Game>()
				.HasMany(g => g.Campaigns)
				.WithOne(c => c.Game)
				.HasForeignKey(c => c.GameId)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<Request>()
				.HasOne(r => r.Campaign)
				.WithMany(c => c.Requests)
				.HasForeignKey(r => r.CampaignId);

			modelBuilder.Entity<Request>()
				.HasOne(r => r.Influencer)
				.WithMany(i => i.Requests)
				.HasForeignKey(r => r.InfluencerId);

			modelBuilder.Entity<Request>()
				.HasOne(r => r.Content)
				.WithMany()
				.HasForeignKey(r => r.ContentId)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<Key>()
				.HasOne(k => k.Request)
				.WithOne(r => r.Key)
				.HasForeignKey<Key>(k => k.RequestId);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.Reviewer)
				.WithMany(d => d.Reviews)
				.HasForeignKey(r => r.ReviewerId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Review>()
				.HasOne(r => r.Reviewee)
				.WithMany(i => i.Reviews)
				.HasForeignKey(r => r.RevieweeId)
				.OnDelete(DeleteBehavior.Restrict);

			base.OnModelCreating(modelBuilder);
		}
	}
}
