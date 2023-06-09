using Uqs.AppointmentBooking.Domain.Services;
using Uqs.AppointmentBooking.Domain.Tests.Unit.Fakes;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class ServicesServiceTests : IDisposable
{
    private readonly ApplicationContextFakeBuilder _ctxBuilder = new();

    private ServicesService? _sut;

    [Fact]
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

    [Fact]
    public async Task GetActiveServices_TwoActiveOneInactiveServices_TwoServices()
    {
        // Arrange
        var ctx = _ctxBuilder
            .WithSingleService(true)
            .WithSingleService(true)
            .WithSingleService(false)
            .Build();
        _sut = new ServicesService(ctx);
        var expected = 2;
        
        // Act
        var actual = await _sut.GetActiveServices();
        
        // Asser
        Assert.Equal(expected, actual.Count());
    }

    public void Dispose()
    {
        _ctxBuilder.Dispose();
    }
}