using APBD_PROJECT.DataLayer;
using APBD_PROJECT.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_PROJECT.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<Software> Software => Set<Software>();
    public DbSet<SoftwareVersion> SoftwareVersions => Set<SoftwareVersion>();
    public DbSet<Discount> Discounts => Set<Discount>();

    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionPayment> SubscriptionPayments => Set<SubscriptionPayment>();

    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>()
            .HasIndex(i => i.Pesel)
            .IsUnique();

        modelBuilder.Entity<Client>()
            .HasIndex(c => c.Krs)
            .IsUnique();


        modelBuilder.Entity<Discount>()
            .Property(d => d.Percentage)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Nowak",
                Pesel = "85010112345",
                CompanyName = null,
                Krs = null,
                Email = "jan.nowak@example.com",
                Phone = "123456789",
                Address = "ul. Przykładowa 1, Warszawa",
                IsSoftDeleted = false
            },
            new Client
            {
                Id = 2,
                FirstName = null,
                LastName = null,
                Pesel = null,
                CompanyName = "Example Company",
                Krs = "0000123456",
                Email = "contact@example.com",
                Phone = "987654321",
                Address = "ul. Firmowa 2, Kraków",
                IsSoftDeleted = false
            }
        );

        modelBuilder.Entity<SoftwareVersion>()
            .HasOne(v => v.Software)
            .WithMany(s => s.Versions)
            .HasForeignKey(v => v.SoftwareId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Contract)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Payment>()
            .HasQueryFilter(p => !p.Contract.Client.IsSoftDeleted);
        
        modelBuilder.Entity<Contract>()
            .HasMany(c => c.Discounts)
            .WithMany()
            .UsingEntity(j => j.ToTable("ContractDiscounts"));
        modelBuilder.Entity<Contract>()
            .HasQueryFilter(c => !c.Client.IsSoftDeleted);

        //UsingEntity will create a table between Contract and Discounts with name "ContractDiscounts"

        modelBuilder.Entity<Subscription>()
            .HasMany(s => s.Discounts)
            .WithMany()
            .UsingEntity(j => j.ToTable("SubscriptionDiscounts"));

        modelBuilder.Entity<Subscription>()
            .HasQueryFilter(s => !s.Client.IsSoftDeleted);


        modelBuilder.Entity<SubscriptionPayment>()
            .HasOne(sp => sp.Subscription)
            .WithMany(s => s.Payments)
            .HasForeignKey(sp => sp.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubscriptionPayment>()
            .HasQueryFilter(sp => !sp.Subscription.Client.IsSoftDeleted);

        //simple filter
        modelBuilder.Entity<Client>()
            .HasQueryFilter(c => !c.IsSoftDeleted);


        modelBuilder.Entity<SoftwareVersion>()
            .Property(v => v.YearlyPrice)
            .HasPrecision(18, 2);


        modelBuilder.Entity<Software>().HasData(
            new Software
            {
                Id = 1,
                Name = "Test Software",
                Description = "A test software",
                Category = "Utilities"
            },
            new Software
            {
                Id = 2,
                Name = "Accounting Suite",
                Description = "Comprehensive accounting software for small businesses",
                Category = "Finance"
            },
            new Software
            {
                Id = 3,
                Name = "Project Manager",
                Description = "Tool for tracking and managing project tasks",
                Category = "Productivity"
            }
        );

        modelBuilder.Entity<SoftwareVersion>().HasData(
            new SoftwareVersion
            {
                Id = 1,
                SoftwareId = 1,
                Version = "1.0.0",
                YearlyPrice = 100m
            }
        );

        modelBuilder.Entity<Discount>().HasData(
            new Discount
            {
                Id = 1,
                Name = "Test Discount",
                Percentage = 10m,
                SoftwareId = 1,
                StartDate = new DateTime(2025, 6, 10),
                EndDate = new DateTime(2026, 6, 10),
                AppliesTo = DiscountTarget.Subscription
            }
        );

        modelBuilder.Entity<Contract>().HasData(
            new Contract
            {
                Id = 1,
                ClientId = 1,
                SoftwareVersionId = 1,
                StartDate = new DateTime(2025, 5, 1),
                EndDate = new DateTime(2025, 6, 1),
                Price = 1000m,
                SupportYears = 1,
                IsSigned = true
            }
        );

        modelBuilder.Entity<Payment>().HasData(
            new Payment
            {
                Id = 1,
                ContractId = 1,
                Amount = 1000m,
                PaymentDate = new DateTime(2025, 5, 27)
            }
        );

        modelBuilder.Entity<Subscription>().HasData(
            new Subscription
            {
                Id = 1,
                ClientId = 2,
                SoftwareId = 1,
                StartDate = new DateTime(2025, 5, 1),
                RenewalPeriod = RenewalPeriod.Monthly,
                Price = 100m,
                IsActive = true
            }
        );

        modelBuilder.Entity<SubscriptionPayment>().HasData(
            new SubscriptionPayment
            {
                Id = 1,
                SubscriptionId = 1,
                Amount = 100m,
                PaymentDate = new DateTime(2025, 5, 20)
            }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = "hash",
                Role = EmployeeRole.User
            }
        );
    }
}