using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace FIXIT.Domain.Entities;
public class User : IdentityUser
{
    [Required, StringLength(80)] public string FirstName { get; set; } = "";
    [Required, StringLength(80)] public string LastName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public CustomerProfile? CustomerProfile { get; set; }
    public TechnicianProfile? TechnicianProfile { get; set; }
    public ICollection<Address> Addresses { get; set; } = [];
    public ICollection<Booking> CustomerBookings { get; set; } = [];
    public ICollection<Booking> TechnicianBookings { get; set; } = [];
}
