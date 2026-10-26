using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FIXIT.Domain.Entities;
namespace FIXIT.Infrastructure;
public class FIXITDbContext(DbContextOptions<FIXITDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
    public DbSet<TechnicianProfile> TechnicianProfiles => Set<TechnicianProfile>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();
    public DbSet<ServiceArea> ServiceAreas => Set<ServiceArea>();
    public DbSet<TechnicianAvailability> TechnicianAvailabilities => Set<TechnicianAvailability>();
    public DbSet<TechnicianSkill> TechnicianSkills => Set<TechnicianSkill>();
    public DbSet<TechnicianCoverageArea> TechnicianCoverageAreas => Set<TechnicianCoverageArea>();
    public DbSet<VerificationRecord> VerificationRecords => Set<VerificationRecord>();
    public DbSet<BookingAssignment> BookingAssignments => Set<BookingAssignment>();
    public DbSet<TechnicianJobDecline> TechnicianJobDeclines => Set<TechnicianJobDecline>();
    public DbSet<PriceLine> PriceLines => Set<PriceLine>();
    public DbSet<AdditionalWorkRequest> AdditionalWorkRequests => Set<AdditionalWorkRequest>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<RefundRequest> RefundRequests => Set<RefundRequest>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<Dispute> Disputes => Set<Dispute>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<PrivacyRequest> PrivacyRequests => Set<PrivacyRequest>();
    public DbSet<BookingPhoto> BookingPhotos => Set<BookingPhoto>();
    public DbSet<PlatformSetting> PlatformSettings => Set<PlatformSetting>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().HasData(
            new Microsoft.AspNetCore.Identity.IdentityRole { Id = "fixit-customer-role", Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "fixit-customer-role" },
            new Microsoft.AspNetCore.Identity.IdentityRole { Id = "fixit-technician-role", Name = "Technician", NormalizedName = "TECHNICIAN", ConcurrencyStamp = "fixit-technician-role" },
            new Microsoft.AspNetCore.Identity.IdentityRole { Id = "fixit-admin-role", Name = "Administrator", NormalizedName = "ADMINISTRATOR", ConcurrencyStamp = "fixit-admin-role" });
        b.Entity<Service>().HasData(
            new Service { Id=1, Name="Air conditioning", Description="Cleaning, inspection, and repair for home air conditioning units.", BasePrice=450, IsActive=true },
            new Service { Id=2, Name="Refrigerator repair", Description="Diagnosis and repair for household refrigerators.", BasePrice=350, IsActive=true },
            new Service { Id=3, Name="Washing machine repair", Description="Troubleshooting and repair for washing machines.", BasePrice=350, IsActive=true },
            new Service { Id=4, Name="Electrical repairs", Description="Minor home electrical repairs and fixture troubleshooting.", BasePrice=400, IsActive=true },
            new Service { Id=5, Name="Plumbing", Description="Leak, drain, and fixture maintenance.", BasePrice=350, IsActive=true },
            new Service { Id=6, Name="Appliance installation", Description="In-home installation of household appliances.", BasePrice=500, IsActive=true },
            new Service { Id=7, Name="Carpentry and furniture", Description="Minor carpentry and furniture repair.", BasePrice=450, IsActive=true },
            new Service { Id=8, Name="Painting", Description="Small-area interior paint touch-ups and maintenance.", BasePrice=500, IsActive=true },
            new Service { Id=9, Name="General home maintenance", Description="General maintenance inspection and minor fixes.", BasePrice=400, IsActive=true });
        b.Entity<ServiceArea>().HasData(
            new ServiceArea { Id=1, CityMunicipality="Makati", Province="Metro Manila", IsActive=true },
            new ServiceArea { Id=2, CityMunicipality="Quezon City", Province="Metro Manila", IsActive=true },
            new ServiceArea { Id=3, CityMunicipality="Pasig", Province="Metro Manila", IsActive=true },
            new ServiceArea { Id=4, CityMunicipality="Cebu City", Province="Cebu", IsActive=true });
        b.Entity<PlatformSetting>().HasData(
            new PlatformSetting { Key="PlatformFeePercent", NumericValue=15m },
            new PlatformSetting { Key="CancellationHours", NumericValue=24m });
        b.Entity<User>().HasOne(x=>x.CustomerProfile).WithOne(x=>x.User).HasForeignKey<CustomerProfile>(x=>x.UserId);
        b.Entity<User>().HasOne(x=>x.TechnicianProfile).WithOne(x=>x.User).HasForeignKey<TechnicianProfile>(x=>x.UserId);
        b.Entity<TechnicianSkill>().HasKey(x=>new{x.TechnicianId,x.ServiceId});
        b.Entity<TechnicianSkill>().HasOne(x=>x.Technician).WithMany(x=>x.TechnicianSkills).HasForeignKey(x=>x.TechnicianId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<TechnicianSkill>().HasOne(x=>x.Service).WithMany().HasForeignKey(x=>x.ServiceId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<TechnicianCoverageArea>().HasKey(x=>new{x.TechnicianId,x.ServiceAreaId});
        b.Entity<TechnicianCoverageArea>().HasOne(x=>x.Technician).WithMany(x=>x.TechnicianCoverageAreas).HasForeignKey(x=>x.TechnicianId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<TechnicianCoverageArea>().HasOne(x=>x.ServiceArea).WithMany().HasForeignKey(x=>x.ServiceAreaId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<User>().HasMany(x=>x.Addresses).WithOne(x=>x.Customer).HasForeignKey(x=>x.CustomerId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Booking>().HasIndex(x=>x.Reference).IsUnique();
        b.Entity<Booking>().HasOne(x=>x.Customer).WithMany(x=>x.CustomerBookings).HasForeignKey(x=>x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Booking>().HasOne(x=>x.Technician).WithMany(x=>x.TechnicianBookings).HasForeignKey(x=>x.TechnicianId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Booking>().HasOne(x=>x.Address).WithMany(x=>x.Bookings).HasForeignKey(x=>x.AddressId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Booking>().HasIndex(x=>new{x.TechnicianId,x.ScheduledAt});
        b.Entity<Service>().Property(x=>x.BasePrice).HasPrecision(10,2);
        b.Entity<Booking>().Property(x=>x.EstimatedTotal).HasPrecision(10,2);
        b.Entity<AdditionalWorkRequest>().Property(x=>x.Amount).HasPrecision(10,2);
        b.Entity<Payment>().Property(x=>x.Amount).HasPrecision(10,2);
        b.Entity<Payment>().Property(x=>x.PlatformFeePercent).HasPrecision(9,2);
        b.Entity<Payment>().Property(x=>x.PlatformFeeAmount).HasPrecision(10,2);
        b.Entity<Payment>().Property(x=>x.TechnicianEarnings).HasPrecision(10,2);
        b.Entity<PriceLine>().Property(x=>x.Amount).HasPrecision(10,2);
        b.Entity<RefundRequest>().Property(x=>x.Amount).HasPrecision(10,2);
        b.Entity<PlatformSetting>().Property(x=>x.NumericValue).HasPrecision(9,2);
        b.Entity<Review>().HasIndex(x=>x.BookingId).IsUnique();
        b.Entity<BookingAssignment>().HasOne(x=>x.Booking).WithMany().HasForeignKey(x=>x.BookingId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<TechnicianJobDecline>().HasOne(x=>x.Booking).WithMany().HasForeignKey(x=>x.BookingId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<TechnicianJobDecline>().HasOne(x=>x.Technician).WithMany().HasForeignKey(x=>x.TechnicianId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<TechnicianJobDecline>().HasIndex(x=>new{x.BookingId,x.TechnicianId}).IsUnique();
        b.Entity<BookingStatusHistory>().HasOne(x=>x.Booking).WithMany(x=>x.History).HasForeignKey(x=>x.BookingId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Payment>().HasIndex(x=>x.BookingId).IsUnique();
        b.Entity<Review>().HasOne(x=>x.Booking).WithOne().HasForeignKey<Review>(x=>x.BookingId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Notification>().HasIndex(x=>new{x.UserId,x.ReadAt});
        b.Entity<AuditLog>().HasIndex(x=>x.CreatedAt);
        b.Entity<ServiceArea>().HasIndex(x=>new{x.CityMunicipality,x.Province}).IsUnique();
        b.Entity<BookingPhoto>().HasOne(x=>x.Booking).WithMany().HasForeignKey(x=>x.BookingId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<PrivacyRequest>().HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
