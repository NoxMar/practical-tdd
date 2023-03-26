using NSubstitute;
using Uqs.AppointmentBooking.Domain.Repositories;
using Uqs.AppointmentBooking.Domain.Services;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class ServicesServiceTests
{
    private readonly IServiceRepository _serviceRepository = Substitute.For<IServiceRepository>();
    private ServicesService? _sut;

    [Fact]
    public async Task GetActiveServices_NoServiceInTheSystem_NoServices()
    {
        // Arrange
        _sut = new ServicesService(_serviceRepository);
        
        // Act
        var actual = await _sut.GetActiveServices();
        
        // Assert
        Assert.Empty(actual);
    }
}