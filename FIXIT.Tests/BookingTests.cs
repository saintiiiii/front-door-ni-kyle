using FIXIT.Domain.Entities;
using FIXIT.Domain;
namespace FIXIT.Tests;
public class BookingTests
{
    [Fact]
    public void NewBookingStartsPendingAndHasPhpEstimateDefault()
    {
        var booking = new Booking();
        Assert.Equal(BookingStatus.Pending, booking.Status);
        Assert.Equal(0m, booking.EstimatedTotal);
        Assert.Empty(booking.Reference);
    }
    [Fact]
    public void TechnicianApplicationsRequireReviewByDefault()
    {
        var profile = new TechnicianProfile();
        Assert.Equal(TechnicianStatus.Pending, profile.Status);
        Assert.False(profile.IsOnline);
    }
    [Theory]
    [InlineData(BookingStatus.Pending, BookingStatus.Accepted, true)]
    [InlineData(BookingStatus.Accepted, BookingStatus.EnRoute, true)]
    [InlineData(BookingStatus.EnRoute, BookingStatus.Arrived, true)]
    [InlineData(BookingStatus.Arrived, BookingStatus.InProgress, true)]
    [InlineData(BookingStatus.InProgress, BookingStatus.WorkCompleted, true)]
    [InlineData(BookingStatus.WorkCompleted, BookingStatus.Completed, true)]
    [InlineData(BookingStatus.Completed, BookingStatus.Disputed, true)]
    [InlineData(BookingStatus.Completed, BookingStatus.InProgress, false)]
    [InlineData(BookingStatus.Cancelled, BookingStatus.Accepted, false)]
    public void BookingWorkflowAcceptsOnlyValidTransitions(BookingStatus from, BookingStatus to, bool expected)
        => Assert.Equal(expected, BookingWorkflow.CanTransition(from, to));
}
