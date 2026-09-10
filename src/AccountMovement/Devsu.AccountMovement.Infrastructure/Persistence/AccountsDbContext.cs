using Devsu.AccountMovement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devsu.AccountMovement.Infrastructure.Persistence;

public class AccountsDbContext : DbContext
{
    public AccountsDbContext(DbContextOptions<AccountsDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<Movement> Movements => Set<Movement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(builder =>
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("account_id").ValueGeneratedOnAdd();

            builder.Property(a => a.AccountNumber).HasColumnName("account_number").HasMaxLength(20).IsRequired();
            builder.Property(a => a.AccountType).HasColumnName("account_type").HasMaxLength(20).IsRequired();
            builder.Property(a => a.InitialBalance).HasColumnName("initial_balance").HasColumnType("decimal(15,2)").IsRequired();
            builder.Property(a => a.Status).HasColumnName("status").IsRequired();
            builder.Property(a => a.CustomerId).HasColumnName("customer_id").IsRequired();

            builder.HasIndex(a => a.AccountNumber).IsUnique();
            builder.HasIndex(a => a.CustomerId);
        });

        modelBuilder.Entity<Movement>(builder =>
        {
            builder.ToTable("Movements");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("movement_id").ValueGeneratedOnAdd();

            builder.Property(m => m.MovementDate).HasColumnName("movement_date").IsRequired();
            builder.Property(m => m.MovementType).HasColumnName("movement_type").HasMaxLength(20).IsRequired();
            builder.Property(m => m.Value).HasColumnName("value").HasColumnType("decimal(15,2)").IsRequired();
            builder.Property(m => m.Balance).HasColumnName("balance").HasColumnType("decimal(15,2)").IsRequired();
            builder.Property(m => m.AccountId).HasColumnName("account_id").IsRequired();

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(m => m.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
