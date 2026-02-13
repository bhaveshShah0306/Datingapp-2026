using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
	public class DataContext : IdentityDbContext<AppUser, AppRole, int,
		IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
		IdentityRoleClaim<int>, IdentityUserToken<int>>
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<UserLike> Likes { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<Connection> Connections { get; set; }
		public DbSet<ProfileImage> ProfileImages { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Group>()
				.HasMany(x => x.Connections)
				.WithOne()
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<AppUser>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(u => u.User)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();

			builder.Entity<AppRole>()
				.HasMany(ur => ur.UserRoles)
				.WithOne(u => u.Role)
				.HasForeignKey(ur => ur.RoleId)
				.IsRequired();


			builder.Entity<UserLike>()
				.HasKey(k => new { k.SourceUserId, k.LikedUserId });

			builder.Entity<UserLike>()
				.HasOne(s => s.SourceUser)
				.WithMany(l => l.LikedUsers)
				.HasForeignKey(s => s.SourceUserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<UserLike>()
				.HasOne(s => s.LikedUser)
				.WithMany(l => l.LikedByUsers)
				.HasForeignKey(s => s.LikedUserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Message>()
				.HasOne(u => u.Recipient)
				.WithMany(m => m.MessagesReceived)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Message>()
				.HasOne(u => u.Sender)
				.WithMany(m => m.MessagesSent)
				.OnDelete(DeleteBehavior.Restrict);

			// ProfileImage Configuration
			builder.Entity<ProfileImage>(entity =>
			{
				entity.ToTable("profile_images");

				entity.HasKey(e => e.Id);
				entity.Property(e => e.Id).HasColumnName("id");

				entity.Property(e => e.FileName)
					.HasColumnName("file_name")
					.HasMaxLength(255);

				entity.Property(e => e.ImageData)
					.HasColumnName("image_data")
					.HasColumnType("bytea")
					.IsRequired();

				entity.Property(e => e.ThumbnailData)
					.HasColumnName("thumbnail_data")
					.HasColumnType("bytea")
					.IsRequired();

				entity.Property(e => e.ContentType)
					.HasColumnName("content_type")
					.HasMaxLength(100);

				entity.Property(e => e.OriginalContentType)
					.HasColumnName("original_content_type")
					.HasMaxLength(100);

				entity.Property(e => e.FileSize)
					.HasColumnName("file_size");

				entity.Property(e => e.OriginalFileSize)
					.HasColumnName("original_file_size");

				entity.Property(e => e.UploadedAt)
					.HasColumnName("uploaded_at")
					.HasDefaultValueSql("CURRENT_TIMESTAMP");

				// Optional: If you want to link to users
				entity.HasOne(p => p.AppUser)
					.WithMany()  // Or WithMany(u => u.ProfileImages) if you want collection
					.HasForeignKey(p => p.AppUserId)
					.IsRequired(false)  // This makes it optional
					.OnDelete(DeleteBehavior.Cascade);
			});


			builder.ApplyUtcDateTimeConverter();
		}
	}

	public static class UtcDateAnnotation
	{
		private const String IsUtcAnnotation = "IsUtc";
		private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
		  new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

		private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
		  new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

		public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
		  builder.HasAnnotation(IsUtcAnnotation, isUtc);

		public static Boolean IsUtc(this IMutableProperty property) =>
		  ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

		/// <summary>
		/// Make sure this is called after configuring all your entities.
		/// </summary>
		public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
		{
			foreach (var entityType in builder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
				{
					if (!property.IsUtc())
					{
						continue;
					}

					if (property.ClrType == typeof(DateTime))
					{
						property.SetValueConverter(UtcConverter);
					}

					if (property.ClrType == typeof(DateTime?))
					{
						property.SetValueConverter(UtcNullableConverter);
					}
				}
			}
		}
	}
}