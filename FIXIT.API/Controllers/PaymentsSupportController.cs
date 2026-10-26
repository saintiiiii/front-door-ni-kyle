using System.Security.Claims;
using FIXIT.API.Contracts;
using FIXIT.Domain.Entities;
using FIXIT.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FIXIT.API.Controllers;
[ApiController,Route("api/payments"),Authorize(Roles="Customer")]
public class PaymentsController(FIXITDbContext db):ControllerBase
{
 string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
 [HttpGet] public async Task<IActionResult> History()=>Ok(await db.Payments.AsNoTracking().Where(p=>p.Booking.CustomerId==UserId).OrderByDescending(p=>p.CreatedAt).Select(p=>new{p.Id,p.TransactionReference,p.Amount,p.PlatformFeePercent,p.PlatformFeeAmount,p.TechnicianEarnings,status=p.Status.ToString(),method=p.Method.ToString(),p.IsSimulation,p.CreatedAt,p.PaidAt,booking=p.Booking.Reference}).ToListAsync());
 [HttpPost("{id:int}/complete-demo")] public async Task<IActionResult> Pay(int id){await using var transaction=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);var p=await db.Payments.Include(x=>x.Booking).SingleOrDefaultAsync(x=>x.BookingId==id&&x.Booking.CustomerId==UserId);if(p is null)return NotFound();if(p.Status==PaymentStatus.Paid)return Conflict(new{message="This payment is already complete."});if(p.Status is PaymentStatus.Refunded or PaymentStatus.Authorized)return Conflict(new{message="This payment cannot be completed in its current state."});if(p.Booking.Status!=BookingStatus.Completed)return Conflict(new{message="Payment becomes available after the customer confirms completed work."});var fee=(await db.PlatformSettings.FindAsync("PlatformFeePercent"))?.NumericValue??15m;p.PlatformFeePercent=fee;p.PlatformFeeAmount=Math.Round(p.Amount*fee/100m,2);p.TechnicianEarnings=p.Amount-p.PlatformFeeAmount;p.Status=PaymentStatus.Paid;p.PaidAt=DateTime.UtcNow;if(p.Booking.TechnicianId is not null)db.Notifications.Add(new(){UserId=p.Booking.TechnicianId,Title="Demo payment finalized",Message=$"Payment for completed booking {p.Booking.Reference} was recorded in the FIXIT demo."});await db.SaveChangesAsync();await transaction.CommitAsync();return Ok(new{p.TransactionReference,p.Amount,p.PlatformFeePercent,p.PlatformFeeAmount,p.TechnicianEarnings,status="Paid",method=p.Method.ToString(),simulation=true,receipt=$"FIXIT demo receipt for {p.Booking.Reference}"});}
 [HttpPost("{id:int}/refund-requests")] public async Task<IActionResult> Refund(int id,[FromBody]TicketRequest r){await using var transaction=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);var p=await db.Payments.SingleOrDefaultAsync(x=>x.Id==id&&x.Booking.CustomerId==UserId);if(p is null)return NotFound();if(p.Status!=PaymentStatus.Paid)return Conflict(new{message="Only completed demo payments may be reviewed for refund."});if(await db.RefundRequests.AnyAsync(x=>x.PaymentId==id&&x.Approved==null))return Conflict(new{message="A refund request for this payment is already awaiting review."});var f=new RefundRequest{PaymentId=id,RequestedById=UserId,Reason=r.Message.Trim(),Amount=p.Amount};db.RefundRequests.Add(f);await db.SaveChangesAsync();await transaction.CommitAsync();return Accepted(new{id=f.Id,status="Pending"});}
}
[ApiController,Route("api/support"),Authorize]
public class SupportController(FIXITDbContext db):ControllerBase
{
 string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
 [HttpGet("tickets")] public async Task<IActionResult> Tickets()=>Ok(await db.SupportTickets.AsNoTracking().Where(t=>t.UserId==UserId).OrderByDescending(t=>t.CreatedAt).Select(t=>new{t.Id,t.Subject,t.Message,status=t.Status.ToString(),t.CreatedAt}).ToListAsync());
 [HttpPost("tickets")] public async Task<IActionResult> Create(TicketRequest r){var t=new SupportTicket{UserId=UserId,Subject=r.Subject.Trim(),Message=r.Message.Trim()};db.Add(t);await db.SaveChangesAsync();return Created($"/api/support/tickets/{t.Id}",new{id=t.Id});}
 [HttpPost("bookings/{id:int}/disputes")] [Authorize(Roles="Customer")] public async Task<IActionResult> Dispute(int id,DisputeRequest r){var b=await db.Bookings.SingleOrDefaultAsync(b=>b.Id==id&&b.CustomerId==UserId);if(b is null)return NotFound();if(b.Status is BookingStatus.Cancelled or BookingStatus.Disputed)return Conflict();b.Status=BookingStatus.Disputed;db.Disputes.Add(new(){BookingId=id,ReportedById=UserId,Description=r.Description.Trim()});db.BookingStatusHistories.Add(new(){BookingId=id,Status=b.Status,ChangedByUserId=UserId,Note="Customer opened dispute"});await db.SaveChangesAsync();return Accepted();}
 [HttpGet("notifications")] public async Task<IActionResult> Notifications()=>Ok(await db.Notifications.AsNoTracking().Where(n=>n.UserId==UserId).OrderByDescending(n=>n.CreatedAt).Take(50).ToListAsync());
 [HttpPost("notifications/{id:int}/read")] public async Task<IActionResult> Read(int id){var n=await db.Notifications.SingleOrDefaultAsync(n=>n.Id==id&&n.UserId==UserId);if(n is null)return NotFound();n.ReadAt=DateTime.UtcNow;await db.SaveChangesAsync();return NoContent();}
}
