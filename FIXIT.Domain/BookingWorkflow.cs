using FIXIT.Domain.Entities;
namespace FIXIT.Domain;
public static class BookingWorkflow
{
    public static bool CanTransition(BookingStatus current, BookingStatus next) => (current,next) switch
    {
        (BookingStatus.Pending, BookingStatus.Accepted or BookingStatus.Cancelled) => true,
        (BookingStatus.Accepted, BookingStatus.EnRoute or BookingStatus.Cancelled) => true,
        (BookingStatus.EnRoute, BookingStatus.Arrived) => true,
        (BookingStatus.Arrived, BookingStatus.InProgress) => true,
        (BookingStatus.InProgress, BookingStatus.WorkCompleted) => true,
        (BookingStatus.WorkCompleted, BookingStatus.Completed or BookingStatus.Disputed) => true,
        (BookingStatus.Completed, BookingStatus.Disputed) => true,
        _ => false
    };
}
