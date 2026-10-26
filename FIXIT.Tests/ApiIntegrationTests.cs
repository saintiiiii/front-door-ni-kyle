using System.Net;
using System.Net.Http.Json;
using FIXIT.Domain.Entities;
using FIXIT.API;
using FIXIT.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace FIXIT.Tests;

public sealed class ApiIntegrationTests : IClassFixture<FixitApiFactory>
{
    private readonly HttpClient _client;
    private readonly FixitApiFactory _factory;
    public ApiIntegrationTests(FixitApiFactory factory)
    {
        _factory=factory;
        _client=factory.CreateClient(new WebApplicationFactoryClientOptions{AllowAutoRedirect=false});
    }

    [Fact]
    public async Task HealthEndpointReportsFixitAndManilaTime()
    {
        var result=await _client.GetFromJsonAsync<Dictionary<string,string>>("/health");
        Assert.Equal("ok",result!["status"]);
        Assert.Equal("FIXIT",result["system"]);
        Assert.Equal("Asia/Manila",result["timeZone"]);
    }

    [Fact]
    public async Task DatabaseSeedPublishesTheNineDemoServices()
    {
        var services=await _client.GetFromJsonAsync<List<object>>("/api/services");
        Assert.NotNull(services);
        Assert.Equal(9,services.Count);
    }

    [Fact]
    public async Task OptionalDemoSeederCreatesFictionalAccountsAndCustomerBookings()
    {
        int pendingBookingId, acceptedBookingId, completedBookingId, paidPaymentId, acceptedPaymentId, refundId;
        int demoAddressId, acceptBookingId;
        string demoTechnicianId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FIXITDbContext>();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            await DemoDataSeeder.SeedAsync(db, users, "LocalDemoPassword123!");
            Assert.Equal(3, await db.Bookings.CountAsync(x => x.Customer.Email == DemoDataSeeder.CustomerEmail));
            Assert.Equal(7, await db.TechnicianAvailabilities.CountAsync(x => x.Technician.Email == DemoDataSeeder.TechnicianEmail));
            Assert.Equal(2, await db.TechnicianCoverageAreas.CountAsync(x => x.TechnicianId == db.Users.Where(u => u.Email == DemoDataSeeder.TechnicianEmail).Select(u => u.Id).Single()));
            pendingBookingId = await db.Bookings.Where(x => x.Customer.Email == DemoDataSeeder.CustomerEmail && x.Status == BookingStatus.Pending).Select(x => x.Id).SingleAsync();
            acceptedBookingId = await db.Bookings.Where(x => x.Customer.Email == DemoDataSeeder.CustomerEmail && x.Status == BookingStatus.Accepted).Select(x => x.Id).SingleAsync();
            completedBookingId = await db.Bookings.Where(x => x.Customer.Email == DemoDataSeeder.CustomerEmail && x.Status == BookingStatus.Completed).Select(x => x.Id).SingleAsync();
            paidPaymentId = await db.Payments.Where(x => x.BookingId == completedBookingId && x.Status == PaymentStatus.Paid).Select(x => x.Id).SingleAsync();
            acceptedPaymentId = await db.Payments.Where(x => x.BookingId == acceptedBookingId).Select(x => x.Id).SingleAsync();
            demoTechnicianId = await db.Users.Where(x => x.Email == DemoDataSeeder.TechnicianEmail).Select(x => x.Id).SingleAsync();
            demoAddressId = await db.Addresses.Where(x => x.Customer.Email == DemoDataSeeder.CustomerEmail).Select(x => x.Id).SingleAsync();
        }

        var csrf = await _client.GetFromJsonAsync<CsrfResponse>("/api/auth/csrf");
        _client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", csrf!.Token);
        var login = await _client.PostAsJsonAsync("/api/auth/login", new { email=DemoDataSeeder.CustomerEmail, password="LocalDemoPassword123!" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var bookings = await _client.GetFromJsonAsync<List<object>>("/api/bookings");
        Assert.Equal(3, bookings!.Count);
        var customerExport = await _client.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/privacy/export");
        Assert.Equal(1, customerExport.GetProperty("data").GetProperty("addresses").GetArrayLength());
        Assert.Equal(3, customerExport.GetProperty("data").GetProperty("customerBookings").GetArrayLength());
        var schedule = DateTimeOffset.UtcNow.AddDays(7).Date.AddHours(10);
        var bookingResponse = await _client.PostAsJsonAsync("/api/bookings", new { serviceId=1, addressId=demoAddressId, problemDescription="Fictional API booking workflow test.", scheduledAt=schedule, paymentMethod=PaymentMethod.SimulatedOnline });
        Assert.Equal(HttpStatusCode.Created, bookingResponse.StatusCode);
        var createdBooking = await bookingResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.Equal(450m, createdBooking.GetProperty("estimatedTotal").GetDecimal());
        var customerBookingId = createdBooking.GetProperty("id").GetInt32();
        var customerNotifications = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/support/notifications");
        Assert.Contains(customerNotifications!, n => n.GetProperty("title").GetString() == "Booking request received");
        var reschedule = await _client.PostAsJsonAsync($"/api/bookings/{customerBookingId}/reschedule", schedule.AddDays(1));
        Assert.Equal(HttpStatusCode.NoContent, reschedule.StatusCode);
        customerNotifications = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/support/notifications");
        Assert.Contains(customerNotifications!, n => n.GetProperty("title").GetString() == "Booking rescheduled");
        var cancellation = await _client.PostAsync($"/api/bookings/{customerBookingId}/cancel", null);
        Assert.Equal(HttpStatusCode.NoContent, cancellation.StatusCode);
        var acceptSchedule = DateTimeOffset.UtcNow.AddDays(12).Date.AddHours(10);
        var offerResponse = await _client.PostAsJsonAsync("/api/bookings", new { serviceId=1, addressId=demoAddressId, problemDescription="Fictional booking for technician acceptance.", scheduledAt=acceptSchedule, paymentMethod=PaymentMethod.CashOnCompletion });
        Assert.Equal(HttpStatusCode.Created, offerResponse.StatusCode);
        acceptBookingId = (await offerResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>()).GetProperty("id").GetInt32();
        var refundRequest = await _client.PostAsJsonAsync($"/api/payments/{paidPaymentId}/refund-requests", new { subject="Fictional demo refund", message="Please review this sample payment." });
        Assert.Equal(HttpStatusCode.Accepted, refundRequest.StatusCode);
        using (var refundBody = await System.Text.Json.JsonDocument.ParseAsync(await refundRequest.Content.ReadAsStreamAsync()))
            refundId = refundBody.RootElement.GetProperty("id").GetInt32();
        var duplicateRefund = await _client.PostAsJsonAsync($"/api/payments/{paidPaymentId}/refund-requests", new { subject="Duplicate request", message="This should be rejected while a request is pending." });
        Assert.Equal(HttpStatusCode.Conflict, duplicateRefund.StatusCode);

        await _client.PostAsync("/api/auth/logout", null);
        var technicianLogin = await _client.PostAsJsonAsync("/api/auth/login", new { email=DemoDataSeeder.TechnicianEmail, password="LocalDemoPassword123!" });
        Assert.Equal(HttpStatusCode.OK, technicianLogin.StatusCode);
        var technicianExport = await _client.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/privacy/export");
        var technicianData = technicianExport.GetProperty("data").GetProperty("technician");
        Assert.Equal(3, technicianData.GetProperty("services").GetArrayLength());
        Assert.Equal(2, technicianData.GetProperty("bookings").GetArrayLength());
        Assert.False(technicianExport.GetRawText().Contains("PrivateStorageKey", StringComparison.OrdinalIgnoreCase));
        var offers = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/technician/jobs");
        Assert.Contains(offers!, x => x.GetProperty("id").GetInt32() == pendingBookingId);
        Assert.Contains(offers!, x => x.GetProperty("id").GetInt32() == acceptBookingId);
        var technicianReviews = await _client.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/technician/reviews");
        Assert.Equal(1, technicianReviews.GetProperty("count").GetInt32());
        Assert.Equal(5, technicianReviews.GetProperty("average").GetDouble());
        var pass = await _client.PostAsJsonAsync($"/api/technician/jobs/{pendingBookingId}/pass", new { reason="Not available for this appointment" });
        Assert.Equal(HttpStatusCode.NoContent, pass.StatusCode);
        var updatedOffers = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/technician/jobs");
        Assert.DoesNotContain(updatedOffers!, x => x.GetProperty("id").GetInt32() == pendingBookingId);
        var acceptOffer = await _client.PostAsync($"/api/technician/jobs/{acceptBookingId}/accept", null);
        Assert.Equal(HttpStatusCode.OK, acceptOffer.StatusCode);
        var acceptedOffers = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/technician/jobs");
        Assert.DoesNotContain(acceptedOffers!, x => x.GetProperty("id").GetInt32() == acceptBookingId);

        foreach (var status in new[] { BookingStatus.EnRoute, BookingStatus.Arrived, BookingStatus.InProgress })
        {
            var update = await _client.PutAsJsonAsync($"/api/technician/jobs/{acceptedBookingId}/status", new { status, note="Fictional demo progress" });
            Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        }
        var report = await _client.PutAsJsonAsync($"/api/technician/jobs/{acceptedBookingId}/work-report", new { completionNotes="Repaired and tested the demo appliance.", partsUsed="One fictional replacement filter" });
        Assert.Equal(HttpStatusCode.OK, report.StatusCode);
        var complete = await _client.PutAsJsonAsync($"/api/technician/jobs/{acceptedBookingId}/status", new { status=BookingStatus.WorkCompleted, note="Work report saved" });
        Assert.Equal(HttpStatusCode.OK, complete.StatusCode);
        await _client.PostAsync("/api/auth/logout", null);
        await _client.PostAsJsonAsync("/api/auth/login", new { email=DemoDataSeeder.CustomerEmail, password="LocalDemoPassword123!" });
        var customerView = await _client.GetFromJsonAsync<System.Text.Json.JsonElement>($"/api/bookings/{acceptedBookingId}");
        Assert.Equal("Repaired and tested the demo appliance.", customerView.GetProperty("completionNotes").GetString());
        Assert.Equal("One fictional replacement filter", customerView.GetProperty("partsUsed").GetString());
        var completion = await _client.PostAsync($"/api/bookings/{acceptedBookingId}/confirm-completion", null);
        Assert.Equal(HttpStatusCode.OK, completion.StatusCode);
        var payment = await _client.PostAsync($"/api/payments/{acceptedBookingId}/complete-demo", null);
        Assert.Equal(HttpStatusCode.OK, payment.StatusCode);
        var paymentResult = await payment.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.Equal(15, paymentResult.GetProperty("platformFeePercent").GetDecimal());
        Assert.Equal(paymentResult.GetProperty("amount").GetDecimal() - paymentResult.GetProperty("platformFeeAmount").GetDecimal(), paymentResult.GetProperty("technicianEarnings").GetDecimal());
        var duplicatePayment = await _client.PostAsync($"/api/payments/{acceptedBookingId}/complete-demo", null);
        Assert.Equal(HttpStatusCode.Conflict, duplicatePayment.StatusCode);
        var settledPayments = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/payments");
        var settledPayment = Assert.Single(settledPayments!, x => x.GetProperty("id").GetInt32() == acceptedPaymentId);
        Assert.Equal("Paid", settledPayment.GetProperty("status").GetString());

        var registration = await _client.PostAsJsonAsync("/api/auth/register", new { email=$"technician-{Guid.NewGuid():N}@example.test", password="LocalDemoPassword123!", firstName="Ari", lastName="Tech", phoneNumber="+639171234567", role="Technician" });
        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);
        var application = await _client.PutAsJsonAsync("/api/technician/application", new { serviceIds=new[] { 5, 6 }, coverageAreaIds=new[] { 1 }, otherSkills="Licensed demo technician" });
        Assert.Equal(HttpStatusCode.OK, application.StatusCode);
        var savedApplication = await _client.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/technician/application");
        Assert.Equal(2, savedApplication.GetProperty("serviceIds").GetArrayLength());

        using (var scope = _factory.Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var admin = new User { UserName="demo.admin@fixit.test", Email="demo.admin@fixit.test", EmailConfirmed=true, FirstName="Demo", LastName="Admin" };
            var created = await users.CreateAsync(admin, "LocalDemoPassword123!");
            Assert.True(created.Succeeded);
            var assigned = await users.AddToRoleAsync(admin, "Administrator");
            Assert.True(assigned.Succeeded);
        }
        await _client.PostAsync("/api/auth/logout", null);
        var adminLogin = await _client.PostAsJsonAsync("/api/auth/login", new { email="demo.admin@fixit.test", password="LocalDemoPassword123!" });
        Assert.Equal(HttpStatusCode.OK, adminLogin.StatusCode);
        var completedAssignment = await _client.PostAsJsonAsync($"/api/admin/bookings/{completedBookingId}/assign", new { technicianId=demoTechnicianId, reason="Test attempted completed-booking reassignment" });
        Assert.Equal(HttpStatusCode.Conflict, completedAssignment.StatusCode);
        var initialAdministrators = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/admin/administrators");
        Assert.Single(initialAdministrators!);
        var createAdministrator = await _client.PostAsJsonAsync("/api/admin/administrators", new { email="second.admin@fixit.test", password="LocalDemoPassword123!", firstName="Second", lastName="Administrator" });
        Assert.Equal(HttpStatusCode.Created, createAdministrator.StatusCode);
        var administrators = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/admin/administrators");
        Assert.Equal(2, administrators!.Count);
        var approveRefund = await _client.PostAsJsonAsync($"/api/admin/refunds/{refundId}/decision", new { approve=true, reason="Approved by integration test." });
        Assert.Equal(HttpStatusCode.NoContent, approveRefund.StatusCode);
        var duplicateDecision = await _client.PostAsJsonAsync($"/api/admin/refunds/{refundId}/decision", new { approve=true, reason="Duplicate decision must not be applied." });
        Assert.Equal(HttpStatusCode.Conflict, duplicateDecision.StatusCode);
        var refunds = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/admin/refunds");
        var finalRefund = Assert.Single(refunds!);
        Assert.Equal("Refunded", finalRefund.GetProperty("paymentStatus").GetString());
        var adminDashboard = await _client.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/admin/dashboard");
        Assert.Equal(paymentResult.GetProperty("platformFeeAmount").GetDecimal(), adminDashboard.GetProperty("platformFees").GetDecimal());
        Assert.Equal(paymentResult.GetProperty("technicianEarnings").GetDecimal(), adminDashboard.GetProperty("technicianEarnings").GetDecimal());
        await _client.PostAsync("/api/auth/logout", null);
        await _client.PostAsJsonAsync("/api/auth/login", new { email=DemoDataSeeder.CustomerEmail, password="LocalDemoPassword123!" });
        var recaptureRefunded = await _client.PostAsync($"/api/payments/{completedBookingId}/complete-demo", null);
        Assert.Equal(HttpStatusCode.Conflict, recaptureRefunded.StatusCode);
        var paymentHistory = await _client.GetFromJsonAsync<List<System.Text.Json.JsonElement>>("/api/payments");
        Assert.Equal("Refunded", Assert.Single(paymentHistory!, x => x.GetProperty("id").GetInt32() == paidPaymentId).GetProperty("status").GetString());
        await _client.PostAsync("/api/auth/logout", null);
        await _client.PostAsJsonAsync("/api/auth/login", new { email=DemoDataSeeder.TechnicianEmail, password="LocalDemoPassword123!" });
        var technicianEarnings = await _client.GetFromJsonAsync<System.Text.Json.JsonElement>("/api/technician/earnings");
        Assert.Equal(1, technicianEarnings.GetProperty("paidJobs").GetInt32());
        Assert.Equal(paymentResult.GetProperty("technicianEarnings").GetDecimal(), technicianEarnings.GetProperty("earnings").GetDecimal());
    }

    [Fact]
    public async Task CustomerCanRegisterButCannotEnterAdminApi()
    {
        var csrf=await _client.GetFromJsonAsync<CsrfResponse>("/api/auth/csrf");
        Assert.False(string.IsNullOrWhiteSpace(csrf!.Token));
        _client.DefaultRequestHeaders.Add("X-XSRF-TOKEN",csrf.Token);
        var registration=await _client.PostAsJsonAsync("/api/auth/register",new{email="demo.customer@example.test",password="DemoCustomer123!",firstName="Demo",lastName="Customer",phoneNumber="+639171234567",role="Customer"});
        Assert.Equal(HttpStatusCode.Created,registration.StatusCode);
        var me=await _client.GetFromJsonAsync<CurrentUser>("/api/auth/me");
        Assert.Equal("demo.customer@example.test",me!.Email);
        Assert.Contains("Customer",me.Roles);
        var admin=await _client.GetAsync("/api/admin/dashboard");
        Assert.Equal(HttpStatusCode.Forbidden,admin.StatusCode);
    }
    [Fact]
    public async Task AddressRecordsAreScopedToTheSignedInCustomer()
    {
        var csrf=await _client.GetFromJsonAsync<CsrfResponse>("/api/auth/csrf");
        _client.DefaultRequestHeaders.Add("X-XSRF-TOKEN",csrf!.Token);
        var firstEmail=$"owner-{Guid.NewGuid():N}@example.test";
        await Register(firstEmail);
        var address=await _client.PostAsJsonAsync("/api/addresses",new{street="Demo Street 1",barangay="Demo Barangay",cityMunicipality="Makati",province="Metro Manila",region="NCR",landmark="",isDefault=true});
        Assert.Equal(HttpStatusCode.Created,address.StatusCode);
        var saved=await address.Content.ReadFromJsonAsync<CreatedAddress>();
        var mine=await _client.GetFromJsonAsync<List<object>>("/api/addresses");
        Assert.Single(mine!);
        await _client.PostAsync("/api/auth/logout",null);
        await Register($"other-{Guid.NewGuid():N}@example.test");
        var theirs=await _client.GetFromJsonAsync<List<object>>("/api/addresses");
        Assert.Empty(theirs!);
        var delete=await _client.DeleteAsync($"/api/addresses/{saved!.Id}");
        Assert.Equal(HttpStatusCode.NotFound,delete.StatusCode);
    }
    private async Task Register(string email)
    {
        var result=await _client.PostAsJsonAsync("/api/auth/register",new{email,password="DemoCustomer123!",firstName="Demo",lastName="Owner",phoneNumber="+639171234567",role="Customer"});
        Assert.Equal(HttpStatusCode.Created,result.StatusCode);
    }
    private sealed record CsrfResponse(string Token);
    private sealed record CurrentUser(string Email,string[] Roles);
    private sealed record CreatedAddress(int Id);
}

public sealed class FixitApiFactory:WebApplicationFactory<Program>
{
    private readonly string _databaseName=$"fixit-integration-{Guid.NewGuid():N}";
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services=>
        {
            services.RemoveAll<DbContextOptions<FIXITDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<FIXITDbContext>>();
            services.RemoveAll<FIXITDbContext>();
            services.AddDbContext<FIXITDbContext>(options=>options.UseInMemoryDatabase(_databaseName).ConfigureWarnings(w=>w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            using var provider=services.BuildServiceProvider();
            using var scope=provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<FIXITDbContext>().Database.EnsureCreated();
        });
    }
}
