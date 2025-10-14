using Microsoft.EntityFrameworkCore;

using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Infrastructure
{
    public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
    {
        internal DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> user = modelBuilder.Entity<User>();

            _ = user.HasKey(e => e.Id);

            _ = user.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasConversion(id => id.Value, value => Uuid.From(value))
                .HasMaxLength(36)
                .IsRequired();

            _ = user.Property(e => e.Email)
                .HasConversion(email => email.Value, value => new EmailAddress(value))
                .HasMaxLength(320)
                .IsRequired();

            _ = user.Property(e => e.FullName)
                .HasConversion(name => name.Value, value => new UserFullName(value))
                .HasMaxLength(100)
                .IsRequired();

            _ = user.Property(e => e.PasswordHash)
                .HasConversion(password => password.Value, hashed => UserPasswordHash.FromHash(hashed))
                .IsRequired();

            _ = user.Property(e => e.CreatedAt)
                .IsRequired();
        }
    }
}
