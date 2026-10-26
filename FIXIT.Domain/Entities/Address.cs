using System.ComponentModel.DataAnnotations;
namespace FIXIT.Domain.Entities;
public class CustomerProfile { [Key] public string UserId { get; set; } = ""; public User User { get; set; } = null!; }
public class Address
{
    public int Id { get; set; }
    [Required, StringLength(200)] public string Street { get; set; } = "";
    [Required, StringLength(100)] public string Barangay { get; set; } = "";
    [Required, StringLength(100)] public string CityMunicipality { get; set; } = "";
    [Required, StringLength(100)] public string Province { get; set; } = "";
    [StringLength(100)] public string Region { get; set; } = "";
    [StringLength(200)] public string? Landmark { get; set; }
    public bool IsDefault { get; set; }
    [Required] public string CustomerId { get; set; } = "";
    public User Customer { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = [];
}
