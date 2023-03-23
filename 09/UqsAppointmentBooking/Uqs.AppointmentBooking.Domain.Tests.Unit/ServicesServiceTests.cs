using Uqs.AppointmentBooking.Domain.Services;
using Uqs.AppointmentBooking.Domain.Tests.Unit.Fakes;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class ServicesServiceTests : IDisposable
{
    private readonly ApplicationContextFakeBuilder _ctxBuilder = new();

    private ServicesService? _sut;

    [Fact(Skip = "TODO")]
    public async Task GetActiveServices_NoServiceInTheSystem_NoServices()
    {
        // Arrange
        var ctx = _ctxBuilder.Build();
        _sut = new ServicesService(ctx);
        
        // Act
        var actual = await _sut.GetActiveServices();
        
        // Assert
        Assert.Empty(actual);
    }

    public void Dispose()
    {
        _ctxBuilder.Dispose();
    }
}