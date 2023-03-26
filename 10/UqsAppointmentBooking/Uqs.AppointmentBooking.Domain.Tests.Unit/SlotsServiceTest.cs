using Microsoft.Extensions.Options;
using NSubstitute;
using Uqs.AppointmentBooking.Domain.Repositories;
using Uqs.AppointmentBooking.Domain.Services;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class SlotsServiceTests
{
    private readonly IServiceRepository _serviceRepository = Substitute.For<IServiceRepository>();
    private readonly IEmployeeRepository _employeeRepository = Substitute.For<IEmployeeRepository>();
    private readonly IAppointmentRepository _appointmentRepository = Substitute.For<IAppointmentRepository>();
    private readonly INowService _nowService = Substitute.For<INowService>();

    private readonly ApplicationSettings _applicationSettings = new()
    {
        OpenAppointmentInDays = 7,
        RoundUpInMin = 5,
        RestInMin = 5
    };

    private readonly IOptions<ApplicationSettings> _settings = Substitute.For<IOptions<ApplicationSettings>>();
    private SlotsService _sut;

    public SlotsServiceTests()
    {
        _settings.Value.Returns(_applicationSettings);
        _sut = new SlotsService(_serviceRepository, _employeeRepository, _appointmentRepository, _nowService,
            _settings);
    }

    [Fact(Skip = "TODO")]
    public async Task GetAvailableSlotsForEmployee_ServiceIdNoFound_ArgumentException()
    {
        // Arrange
        
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _sut.GetAvailableSlotsForEmployee("AServiceId", "AEmployeeId"));
        
        // Assert
        Assert.IsType<ArgumentException>(exception);
    }
}