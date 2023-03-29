using FluentAssertions;
using NSubstitute;
using Uqs.AppointmentBooking.Domain.DomainObjects;
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
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveServices_TwoActiveServices_TwoServices()
    {
        // Arrange
        _serviceRepository.GetActiveServices()
            .Returns(new[]
            {
                new Service { IsActive = true },
                new Service { IsActive = true }
            });
        _sut = new ServicesService(_serviceRepository);
        var expected = 2;
        
        // Act
        var actual = (await _sut.GetActiveServices()).ToArray();
        
        // Assert
        actual.Length.Should().Be(expected);
    }
}