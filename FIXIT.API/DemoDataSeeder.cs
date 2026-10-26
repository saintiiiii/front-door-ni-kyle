using FIXIT.Domain.Entities;
using FIXIT.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FIXIT.API;

/// <summary>Creates fictional local demonstration accounts and bookings when explicitly enabled.</summary>
public static class DemoDataSeeder
{
    public const string CustomerEmail = "demo.customer@fixit.test";
    public const string TechnicianEmail = "demo.technician@fixit.test";

    public static async Task SeedAsync(FIXITDbContext db, UserManager<User> users, string password)
    {
        var customer = await FindOrCreateUser(users, CustomerEmail, "Mika", "Santos", "+639171000001", password);
        var technician = await FindOrCreateUser(users, TechnicianEmail, "Paolo", "Reyes", "+639171000002", password);
        await EnsureRole(users, customer, "Customer");
        await EnsureRole(users, technician, "Technician");

        if (!await db.CustomerProfiles.AnyAsync(x => x.UserId == customer.Id))
            db.CustomerProfiles.Add(new CustomerProfile { UserId = customer.Id });

        var techProfile = await db.TechnicianProfiles.SingleOrDefaultAsync(x => x.UserId == technician.Id);
        if (techProfile is null)
        {
            db.TechnicianProfiles.Add(new TechnicianProfile
            {
                UserId = technician.Id,
                Skills = "Residential appliance maintenance",
                CoverageAreas = "Makati, Pasig",
                Status = TechnicianStatus.Approved,
                IsOnline = true,
                DecisionReason = "Fictional, pre-approved local demo account"
            });
        }
        else
        {
            techProfile.Skills = "Residential appliance maintenance";
            techProfile.CoverageAreas = "Makati, Pasig";
            techProfile.Status = TechnicianStatus.Approved;
            techProfile.IsOnline = true;
            techProfile.DecisionReason = "Fictional, pre-approved local demo account";
        }
        var existingSkillIds = await db.TechnicianSkills.Where(x => x.TechnicianId == technician.Id).Select(x => x.ServiceId).ToListAsync();
        foreach (var serviceId in new[] { 1, 2, 3 }.Except(existingSkillIds))
            db.TechnicianSkills.Add(new TechnicianSkill { TechnicianId = technician.Id, ServiceId = serviceId });
        var existingAreaIds = await db.TechnicianCoverageAreas.Where(x => x.TechnicianId == technician.Id).Select(x => x.ServiceAreaId).ToListAsync();
        foreach (var areaId in new[] { 1, 3 }.Except(existingAreaIds))
            db.TechnicianCoverageAreas.Add(new TechnicianCoverageArea { TechnicianId = technician.Id, ServiceAreaId = areaId });
        if (!await db.TechnicianAvailabilities.AnyAsync(x => x.TechnicianId == technician.Id))
        {
            for (var day = 0; day < 7; day++)
                db.TechnicianAvailabilities.Add(new TechnicianAvailability
                {
                    TechnicianId = technician.Id,
                    DayOfWeek = day,
                    StartTime = new TimeOnly(0, 0),
                    EndTime = new TimeOnly(23, 59),
                    IsAvailable = true
                });
        }

        if (!await db.Addresses.AnyAsync(x => x.CustomerId == customer.Id))
        {
            db.Addresses.Add(new Address
            {
                CustomerId = customer.Id,
                Street = "100 Demo Street",
                Barangay = "Poblacion",
                CityMunicipality = "Makati",
                Province = "Metro Manila",
                Region = "NCR",
                Landmark = "Near the demo community center",
                IsDefault = true
            });
            await db.SaveChangesAsync();
        }

        if (await db.Bookings.AnyAsync(x => x.CustomerId == customer.Id)) return;

        var address = await db.Addresses.FirstAsync(x => x.CustomerId == customer.Id);
        var services = await db.Services.Where(x => x.IsActive).OrderBy(x => x.Id).Take(3).ToListAsync();
        if (services.Count < 3) throw new InvalidOperationException("FIXIT demo services are missing. Apply the database migration first.");

        var now = DateTime.UtcNow;
        var feePercent = (await db.PlatformSettings.FindAsync("PlatformFeePercent"))?.NumericValue ?? 15m;
        var examples = new[]
        {
            new { Ref = "FX-DEMO-001", Service = services[0], When = now.AddDays(3), Status = BookingStatus.Pending, TechnicianId = (string?)null, PaymentStatus = PaymentStatus.Pending },
            new { Ref = "FX-DEMO-002", Service = services[1], When = now.AddDays(5), Status = BookingStatus.Accepted, TechnicianId = (string?)technician.Id, PaymentStatus = PaymentStatus.Pending },
            new { Ref = "FX-DEMO-003", Service = services[2], When = now.AddDays(-2), Status = BookingStatus.Completed, TechnicianId = (string?)technician.Id, PaymentStatus = PaymentStatus.Paid }
        };

        foreach (var example in examples)
        {
            var booking = new Booking
            {
                Reference = example.Ref,
                CustomerId = customer.Id,
                TechnicianId = example.TechnicianId,
                AddressId = address.Id,
                ServiceId = example.Service.Id,
                ProblemDescription = $"Fictional demo request for {example.Service.Name.ToLowerInvariant()}.",
                ScheduledAt = example.When,
                Status = example.Status,
                EstimatedTotal = example.Service.BasePrice
            };
            db.Bookings.Add(booking);
            db.PriceLines.Add(new PriceLine { Booking = booking, Label = example.Service.Name, Amount = example.Service.BasePrice });
            db.Payments.Add(new Payment
            {
                Booking = booking,
                Method = PaymentMethod.SimulatedOnline,
                Amount = example.Service.BasePrice,
                Status = example.PaymentStatus,
                PlatformFeePercent = example.PaymentStatus == PaymentStatus.Paid ? feePercent : 0m,
                PlatformFeeAmount = example.PaymentStatus == PaymentStatus.Paid ? Math.Round(example.Service.BasePrice * feePercent / 100m, 2) : 0m,
                TechnicianEarnings = example.PaymentStatus == PaymentStatus.Paid ? example.Service.BasePrice - Math.Round(example.Service.BasePrice * feePercent / 100m, 2) : 0m,
                PaidAt = example.PaymentStatus == PaymentStatus.Paid ? example.When : null,
                IsSimulation = true
            });
            db.BookingStatusHistories.Add(new BookingStatusHistory
            {
                Booking = booking,
                Status = BookingStatus.Pending,
                ChangedByUserId = customer.Id,
                ChangedAt = example.Status == BookingStatus.Completed ? example.When.AddMinutes(-40) : now,
                Note = "Fictional seeded demonstration booking"
            });
            if (example.TechnicianId is not null)
            {
                db.BookingAssignments.Add(new BookingAssignment
                {
                    Booking = booking,
                    TechnicianId = technician.Id,
                    AssignedById = technician.Id,
                    AssignedAt = example.Status == BookingStatus.Completed ? example.When.AddMinutes(-30) : now,
                    Note = "Fictional seeded demonstration assignment"
                });
                db.BookingStatusHistories.Add(new BookingStatusHistory
                {
                    Booking = booking,
                    Status = BookingStatus.Accepted,
                    ChangedByUserId = technician.Id,
                    ChangedAt = example.Status == BookingStatus.Completed ? example.When.AddMinutes(-30) : now.AddSeconds(1),
                    Note = "Fictional seeded demonstration assignment"
                });
            }
            if (example.Status == BookingStatus.Completed)
            {
                db.Reviews.Add(new Review { Booking = booking, CustomerId = customer.Id, Rating = 5, Comment = "Fictional demo review: clear communication and careful repair." });
                db.BookingStatusHistories.Add(new BookingStatusHistory
                {
                    Booking = booking,
                    Status = BookingStatus.WorkCompleted,
                    ChangedByUserId = technician.Id!,
                    ChangedAt = example.When.AddMinutes(-10),
                    Note = "Fictional demo repair completed"
                });
                db.BookingStatusHistories.Add(new BookingStatusHistory
                {
                    Booking = booking,
                    Status = BookingStatus.Completed,
                    ChangedByUserId = customer.Id,
                    ChangedAt = example.When,
                    Note = "Fictional demo completion confirmed"
                });
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task<User> FindOrCreateUser(UserManager<User> users, string email, string firstName, string lastName, string phone, string password)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is not null) return user;

        user = new User
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone
        };
        var result = await users.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException("Unable to create configured FIXIT demo accounts. Check the demo password policy.");
        return user;
    }

    private static async Task EnsureRole(UserManager<User> users, User user, string role)
    {
        if (!await users.IsInRoleAsync(user, role))
        {
            var result = await users.AddToRoleAsync(user, role);
            if (!result.Succeeded) throw new InvalidOperationException($"Unable to add the FIXIT demo account to the {role} role.");
        }
    }
}
