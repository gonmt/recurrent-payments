using Microsoft.EntityFrameworkCore;
using Payments.Core.Shared.Domain.ValueObjects;
using Payments.Core.Users.Domain;

namespace Payments.Core.Users.Infrastructure;

public sealed class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    internal DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var user = modelBuilder.Entity<User>();

        user.HasKey(e => e.Id);

        user.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => Uuid.From(value))
            .HasMaxLength(36)
            .IsRequired();

        user.Property(e => e.Email)
            .HasConversion(email => email.Value, value => new EmailAddress(value))
            .HasMaxLength(320)
            .IsRequired();

        user.Property(e => e.FullName)
            .HasConversion(name => name.Value, value => new UserFullName(value))
            .HasMaxLength(100)
            .IsRequired();

        user.Property(e => e.PasswordHash)
            .HasConversion(password => password.Value, hashed => UserPasswordHash.FromHash(hashed))
            .IsRequired();

        user.Property(e => e.CreatedAt)
            .IsRequired();
    }
}
