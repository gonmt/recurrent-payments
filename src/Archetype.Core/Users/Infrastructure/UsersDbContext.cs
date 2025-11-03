using Archetype.Core.Shared.Domain.ValueObjects;
using Archetype.Core.Users.Domain;

using Microsoft.EntityFrameworkCore;

namespace Archetype.Core.Users.Infrastructure;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    internal DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> user = modelBuilder.Entity<User>();

        _ = user.ToTable("users");

        _ = user.HasKey(e => e.Id)
            .HasName("pk_users");

        _ = user.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => Uuid.From(value))
            .HasColumnName("id")
            .HasMaxLength(36)
            .IsRequired();

        user.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(320)
                .IsRequired();
        });
        user.Navigation(u => u.Email)
            .IsRequired();

        user.OwnsOne(u => u.FullName, fullName =>
        {
            fullName.Property(f => f.Value)
                .HasColumnName("full_name")
                .HasMaxLength(100)
                .IsRequired();
        });
        user.Navigation(u => u.FullName)
            .IsRequired();

        _ = user.Property<UserPasswordHash>("_passwordHash")
            .HasConversion(password => password.Value, hashed => UserPasswordHash.FromHash(hashed))
            .HasColumnName("password_hash")
            .IsRequired();

        _ = user.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}
