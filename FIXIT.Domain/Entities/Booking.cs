using System.ComponentModel.DataAnnotations;
namespace FIXIT.Domain.Entities;
public class Service
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, StringLength(2000)] public string Description { get; set; } = "";
    [Range(0, 100000)] public decimal BasePrice { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Booking> Bookings { get; set; } = [];
}
public class Booking
{
    public int Id { get; set; }
    [Required, StringLength(30)] public string Reference { get; set; } = "";
    [Required] public string CustomerId { get; set; } = "";
    public User Customer { get; set; } = null!;
    public string? TechnicianId { get; set; }
    public User? Technician { get; set; }
    public int AddressId { get; set; }
    public Address Address { get; set; } = null!;
    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    [Required, StringLength(2000)] public string ProblemDescription { get; set; } = "";
    public DateTime ScheduledAt { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    [Range(0, 100000)] public decimal EstimatedTotal { get; set; }
    [StringLength(4000)] public string? CompletionNotes { get; set; }
    [StringLength(2000)] public string? PartsUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<BookingStatusHistory> History { get; set; } = [];
}
public enum BookingStatus { Pending, Accepted, EnRoute, Arrived, InProgress, WorkCompleted, Completed, Cancelled, Disputed }
public class BookingStatusHistory
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public BookingStatus Status { get; set; }
    [Required, StringLength(450)] public string ChangedByUserId { get; set; } = "";
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    [StringLength(500)] public string? Note { get; set; }
}
