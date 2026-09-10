using Devsu.PersonCustomer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devsu.PersonCustomer.Infrastructure.Persistence;

public class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(builder =>
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("client_id").ValueGeneratedOnAdd();

            builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            builder.Property(c => c.Gender).HasColumnName("gender").HasMaxLength(20).IsRequired();
            builder.Property(c => c.Age).HasColumnName("age").IsRequired();
            builder.Property(c => c.Identification).HasColumnName("identification").HasMaxLength(20).IsRequired();
            builder.Property(c => c.Address).HasColumnName("address").HasMaxLength(200).IsRequired();
            builder.Property(c => c.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
            builder.Property(c => c.Password).HasColumnName("password").HasMaxLength(100).IsRequired();
            builder.Property(c => c.Status).HasColumnName("status").IsRequired();

            builder.HasIndex(c => c.Identification).IsUnique();
        });
    }
}
