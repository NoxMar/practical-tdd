using Microsoft.EntityFrameworkCore;
using Uqs.AppointmentBooking.Domain.Database;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class ApplicationContextFake : ApplicationContext
{
    public ApplicationContextFake() : base(
        new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: $"AAppointmentBookingTest{Guid.NewGuid()}")
            .Options)
    {
    }
}