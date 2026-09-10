using System;
using System.Collections.Generic;
using Devsu.AccountMovement.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devsu.AccountMovement.Infrastructure.Persistence;

public partial class AccountsDbContext : DbContext
{
    public AccountsDbContext(DbContextOptions<AccountsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Movement> Movements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasIndex(e => e.CustomerId, "IX_Accounts_CustomerId");

            entity.HasIndex(e => e.AccountNumber, "UQ_Accounts_AccountNumber").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("account_number");
            entity.Property(e => e.AccountType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("account_type");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.InitialBalance)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("initial_balance");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.Property(e => e.MovementId).HasColumnName("movement_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Balance)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.MovementDate).HasColumnName("movement_date");
            entity.Property(e => e.MovementType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("movement_type");
            entity.Property(e => e.Value)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("value");

            entity.HasOne(d => d.Account).WithMany(p => p.Movements)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movements_Accounts");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
