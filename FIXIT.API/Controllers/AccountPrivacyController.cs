using FIXIT.API.Contracts;
using FIXIT.Domain.Entities;
using FIXIT.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace FIXIT.API.Controllers;
[ApiController,Route("api/account"),Authorize]
public class AccountController(FIXITDbContext db):ControllerBase
{
 string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
 [HttpGet("profile")] public async Task<IActionResult> Profile(){var u=await db.Users.FindAsync(UserId);return u is null?NotFound():Ok(new{u.Email,u.PhoneNumber,u.FirstName,u.LastName});}
 [HttpPut("profile")] public async Task<IActionResult> UpdateProfile(ProfileUpdate r){var u=await db.Users.FindAsync(UserId);if(u is null)return NotFound();u.FirstName=r.FirstName.Trim();u.LastName=r.LastName.Trim();u.PhoneNumber=r.PhoneNumber.Trim();await db.SaveChangesAsync();return NoContent();}
 [HttpGet("security")] public IActionResult Security()=>Ok(new{passwordChangeAvailable=true,accountDeletionAvailable=true,privacyContact="privacy@fixit.example",timeZone="Asia/Manila"});
}
[ApiController,Route("api/privacy"),Authorize]
public class PrivacyController(FIXITDbContext db):ControllerBase
{
 string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
 [HttpGet("export")]
 public async Task<IActionResult> Export()
 {
     var user=await db.Users.AsNoTracking().Where(x=>x.Id==UserId).Select(x=>new{x.Email,x.PhoneNumber,x.FirstName,x.LastName,x.CreatedAt}).SingleOrDefaultAsync();
     if(user is null)return NotFound();
     var addresses=await db.Addresses.AsNoTracking().Where(x=>x.CustomerId==UserId).Select(a=>new{a.Street,a.Barangay,a.CityMunicipality,a.Province,a.Region,a.Landmark,a.IsDefault}).ToListAsync();
     var customerBookings=await db.Bookings.AsNoTracking().Where(b=>b.CustomerId==UserId).OrderByDescending(b=>b.CreatedAt).Select(b=>new{b.Reference,b.ProblemDescription,b.ScheduledAt,status=b.Status.ToString(),b.EstimatedTotal,service=b.Service.Name,address=new{b.Address.Street,b.Address.Barangay,b.Address.CityMunicipality,b.Address.Province,b.Address.Region,b.Address.Landmark},technician=b.Technician==null?null:new{b.Technician.FirstName,b.Technician.LastName},payment=db.Payments.Where(p=>p.BookingId==b.Id).Select(p=>new{p.Amount,status=p.Status.ToString(),method=p.Method.ToString(),p.TransactionReference,p.PlatformFeePercent,p.PlatformFeeAmount,p.IsSimulation}).FirstOrDefault()}).ToListAsync();
     var techProfile=await db.TechnicianProfiles.AsNoTracking().Where(x=>x.UserId==UserId).Select(x=>new{x.Skills,status=x.Status.ToString(),x.CoverageAreas,x.DecisionReason,x.ReviewedAt,x.IsOnline}).SingleOrDefaultAsync();
     object? technician=null;
     if(techProfile is not null)
     {
         var services=await db.TechnicianSkills.AsNoTracking().Where(x=>x.TechnicianId==UserId).Select(x=>x.Service.Name).ToListAsync();
         var coverageAreas=await db.TechnicianCoverageAreas.AsNoTracking().Where(x=>x.TechnicianId==UserId).Select(x=>new{x.ServiceArea.CityMunicipality,x.ServiceArea.Province}).ToListAsync();
         var availability=await db.TechnicianAvailabilities.AsNoTracking().Where(x=>x.TechnicianId==UserId).OrderBy(x=>x.DayOfWeek).Select(x=>new{x.DayOfWeek,x.StartTime,x.EndTime,x.IsAvailable}).ToListAsync();
         var verification=await db.VerificationRecords.AsNoTracking().Where(x=>x.TechnicianId==UserId).OrderByDescending(x=>x.SubmittedAt).Select(x=>new{x.DocumentType,status=x.Status.ToString(),x.DecisionReason,x.SubmittedAt,x.ReviewedAt}).ToListAsync();
         var technicianBookings=await db.Bookings.AsNoTracking().Where(x=>x.TechnicianId==UserId).OrderByDescending(x=>x.ScheduledAt).Select(x=>new{x.Reference,x.ScheduledAt,status=x.Status.ToString(),service=x.Service.Name,x.ProblemDescription,x.CompletionNotes,x.PartsUsed,x.EstimatedTotal}).ToListAsync();
         technician=new{techProfile.Skills,services,coverageAreas,status=techProfile.status,techProfile.DecisionReason,techProfile.ReviewedAt,techProfile.IsOnline,availability,verification,bookings=technicianBookings};
     }
     var payments=await db.Payments.AsNoTracking().Where(x=>x.Booking.CustomerId==UserId).OrderByDescending(x=>x.CreatedAt).Select(x=>new{x.Booking.Reference,x.Amount,status=x.Status.ToString(),method=x.Method.ToString(),x.TransactionReference,x.PlatformFeePercent,x.PlatformFeeAmount,x.IsSimulation,x.CreatedAt,x.PaidAt}).ToListAsync();
     var refunds=await db.RefundRequests.AsNoTracking().Where(x=>x.RequestedById==UserId).OrderByDescending(x=>x.CreatedAt).Select(x=>new{booking=x.Payment.Booking.Reference,x.Amount,x.Reason,x.Approved,x.CreatedAt}).ToListAsync();
     var reviewsSubmitted=await db.Reviews.AsNoTracking().Where(x=>x.CustomerId==UserId).OrderByDescending(x=>x.CreatedAt).Select(x=>new{booking=x.Booking.Reference,x.Rating,x.Comment,state=x.State.ToString(),x.CreatedAt}).ToListAsync();
     var reviewsReceived=await db.Reviews.AsNoTracking().Where(x=>x.Booking.TechnicianId==UserId).OrderByDescending(x=>x.CreatedAt).Select(x=>new{booking=x.Booking.Reference,service=x.Booking.Service.Name,x.Rating,x.Comment,state=x.State.ToString(),x.CreatedAt}).ToListAsync();
     var tickets=await db.SupportTickets.AsNoTracking().Where(x=>x.UserId==UserId).OrderByDescending(x=>x.CreatedAt).Select(x=>new{x.Subject,x.Message,status=x.Status.ToString(),x.CreatedAt}).ToListAsync();
     var privacyRequests=await db.PrivacyRequests.AsNoTracking().Where(x=>x.UserId==UserId).OrderByDescending(x=>x.CreatedAt).Select(x=>new{x.Reason,x.Approved,x.CreatedAt,x.ReviewedAt}).ToListAsync();
     var notifications=await db.Notifications.AsNoTracking().Where(x=>x.UserId==UserId).OrderByDescending(x=>x.CreatedAt).Select(x=>new{x.Title,x.Message,x.CreatedAt,x.ReadAt}).ToListAsync();
     var photos=await db.BookingPhotos.AsNoTracking().Where(x=>x.UploadedById==UserId).OrderByDescending(x=>x.UploadedAt).Select(x=>new{booking=x.Booking.Reference,x.ContentType,x.Phase,x.UploadedAt}).ToListAsync();
     var data=new{account=user,addresses,customerBookings,technician,payments,refunds,reviewsSubmitted,reviewsReceived,tickets,privacyRequests,notifications,photos};
     return Ok(new{notice="FIXIT data export. Includes personal account, customer and technician records available to this account. Verification file contents and private storage paths are excluded.",exportedAt=DateTimeOffset.UtcNow,data});
 }
 [HttpPost("deletion-requests")] public async Task<IActionResult> RequestDeletion([FromBody]TicketRequest r){if(await db.PrivacyRequests.AnyAsync(x=>x.UserId==UserId&&x.Approved==null))return Conflict(new{message="A deletion request is already being reviewed."});var x=new PrivacyRequest{UserId=UserId,Reason=r.Message.Trim()};db.Add(x);await db.SaveChangesAsync();return Accepted(new{id=x.Id,status="Pending review",retention="Bookings may be retained for financial and dispute records, with identifying account and address details removed."});}
}
