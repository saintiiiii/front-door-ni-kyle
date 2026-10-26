using FIXIT.API.Contracts;
using FIXIT.Domain.Entities;
using FIXIT.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FIXIT.API.Controllers;
[ApiController,Route("api/addresses"),Authorize(Roles="Customer")]
public class AddressesController(FIXITDbContext db) : ControllerBase
{
    string UserId=>User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
    [HttpGet] public async Task<IActionResult> List()=>Ok(await db.Addresses.AsNoTracking().Where(x=>x.CustomerId==UserId).OrderByDescending(x=>x.IsDefault).Select(a=>new{a.Id,a.Street,a.Barangay,a.CityMunicipality,a.Province,a.Region,a.Landmark,a.IsDefault}).ToListAsync());
    [HttpPost] public async Task<IActionResult> Create(AddressRequest r){if(r.IsDefault){var existing=await db.Addresses.Where(x=>x.CustomerId==UserId).ToListAsync();foreach(var item in existing)item.IsDefault=false;}var a=new Address{CustomerId=UserId,Street=r.Street.Trim(),Barangay=r.Barangay.Trim(),CityMunicipality=r.CityMunicipality.Trim(),Province=r.Province.Trim(),Region=r.Region.Trim(),Landmark=r.Landmark?.Trim(),IsDefault=r.IsDefault};db.Addresses.Add(a);await db.SaveChangesAsync();return Created($"/api/addresses/{a.Id}",new{id=a.Id,a.Street,a.Barangay,a.CityMunicipality,a.Province,a.Region,a.Landmark,a.IsDefault});}
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){var a=await db.Addresses.SingleOrDefaultAsync(x=>x.Id==id&&x.CustomerId==UserId);if(a is null)return NotFound();if(await db.Bookings.AnyAsync(x=>x.AddressId==id&&x.Status!=BookingStatus.Cancelled))return Conflict(new{message="This address is used by a booking."});db.Remove(a);await db.SaveChangesAsync();return NoContent();}
}
