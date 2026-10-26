using FIXIT.API.Contracts;
using FIXIT.Domain.Entities;
using FIXIT.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
namespace FIXIT.API.Controllers;
[ApiController,Route("api/auth")]
public class AuthController(UserManager<User> users,SignInManager<User> signIn,FIXITDbContext db,IAntiforgery antiforgery) : ControllerBase
{
    [HttpGet("csrf"),AllowAnonymous]
    public IActionResult Csrf(){var t=antiforgery.GetAndStoreTokens(HttpContext);return Ok(new{token=t.RequestToken});}
    [HttpPost("register"),AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest r)
    {
        var role=r.Role.Trim();if(role is not ("Customer" or "Technician"))return BadRequest(new{message="Choose Customer or Technician."});
        if(await users.FindByEmailAsync(r.Email) is not null)return Conflict(new{message="An account with that email already exists."});
        var phone=Regex.Replace(r.PhoneNumber,@"[\s()-]","");if(!Regex.IsMatch(phone,@"^(?:\+63|0)9\d{9}$"))return BadRequest(new{message="Enter a valid Philippine mobile number, such as +639171234567."});
        var u=new User{UserName=r.Email,Email=r.Email,PhoneNumber=phone,FirstName=r.FirstName.Trim(),LastName=r.LastName.Trim(),EmailConfirmed=false};
        var made=await users.CreateAsync(u,r.Password);if(!made.Succeeded)return BadRequest(new{errors=made.Errors.Select(e=>e.Description)});
        await users.AddToRoleAsync(u,role);
        if(role=="Customer")db.CustomerProfiles.Add(new CustomerProfile{UserId=u.Id});
        else db.TechnicianProfiles.Add(new TechnicianProfile{UserId=u.Id,Skills="",CoverageAreas=""});
        await db.SaveChangesAsync();await signIn.SignInAsync(u,isPersistent:false);
        return Created("/api/auth/me",new{u.Id,u.Email,u.FirstName,u.LastName,role,technicianStatus=role=="Technician"?TechnicianStatus.Pending.ToString():null});
    }
    [HttpPost("login"),AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest r){var u=await users.FindByEmailAsync(r.Email);if(u is null)return Unauthorized(new{message="Email or password is incorrect."});var result=await signIn.PasswordSignInAsync(u,r.Password,true,true);if(!result.Succeeded)return Unauthorized(new{message="Email or password is incorrect, or the account is locked."});return Ok(new{u.Id,u.Email,u.FirstName,u.LastName,roles=await users.GetRolesAsync(u)});}
    [HttpPost("logout"),Authorize]
    public async Task<IActionResult> Logout(){await signIn.SignOutAsync();return NoContent();}
    [HttpPost("change-password"),Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest r){var u=await users.GetUserAsync(User);if(u is null)return Unauthorized();var result=await users.ChangePasswordAsync(u,r.CurrentPassword,r.NewPassword);if(!result.Succeeded)return BadRequest(new{errors=result.Errors.Select(e=>e.Description)});await signIn.RefreshSignInAsync(u);return NoContent();}
    [HttpGet("me"),Authorize]
    public async Task<IActionResult> Me(){var u=await users.GetUserAsync(User);if(u is null)return Unauthorized();var roles=await users.GetRolesAsync(u);var tech=await db.TechnicianProfiles.FindAsync(u.Id);return Ok(new{id=u.Id,u.Email,u.PhoneNumber,u.FirstName,u.LastName,roles,technicianStatus=tech?.Status.ToString()});}
}
