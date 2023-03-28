using Microsoft.Extensions.Options;
using NSubstitute;
using Uqs.AppointmentBooking.Domain.DomainObjects;
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

    [Fact]
    public async Task GetAvailableSlotsForEmployee_ServiceIdNoFound_ArgumentException()
    {
        // Arrange
        _employeeRepository.GetItemAsync("AEmployeeId")
            .Returns(new Employee{Id = "AEmployeeId"});
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _sut.GetAvailableSlotsForEmployee("AServiceId", "AEmployeeId"));
        
        // Assert
        var ex = Assert.IsType<ArgumentException>(exception);
        Assert.Equal("serviceId", ex.ParamName);
    }
    
    [Fact]
    public async Task GetAvailableSlotsForEmployee_EmployeeNotFound_ArgumentException()
    {
        // Arrange
        _serviceRepository.GetActiveService("AServiceId")
            .Returns(new Service { Id = "AServiceId", IsActive = true });
        
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.GetAvailableSlotsForEmployee("AServiceId", "AEmployeeId"));
        
        // Assert
        var ex = Assert.IsType<ArgumentException>(exception);
        Assert.Equal("employeeId", ex.ParamName);
    }

    [Fact]
    public async Task GetAvailableSlotsForEmployee_NoShiftsForTomAndNoAppointmentsInSystem_NoSlots()
    {
        // Arrange
        var appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        var tom = new Employee { Id = "Tom", Name = "Thomas Fringe", Shifts = Array.Empty<Shift>() };
        var mensCut30Min = new Service { Id = "MensCut30Min", AppointmentTimeSpanInMin = 30 };
        _serviceRepository.GetActiveService(Arg.Any<String>()).Returns(mensCut30Min);
        _employeeRepository.GetItemAsync(Arg.Any<String>()).Returns(tom);
        
        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30Min.Id, tom.Id);
        
        // Assert
        var times = slots.DaysSlots.SelectMany(s => s.Times);
        Assert.Empty(times);
    }

    [Theory]
    [InlineData(5, 0)]
    [InlineData(25, 0)]
    [InlineData(30, 1, "2022-10-03 09:00:00")]
    [InlineData(35, 2, "2022-10-03 09:00:00", "2022-10-03 09:05:00")]
    public async Task GetAvailableSlotsForEmployee_OneShiftAndNoExistingAppointments_VaryingSlots(int serviceDuration,
        int totalSlots, params string[] expectedTimes)
    {
        // Arrange
        DateTime appointmentFrom = new(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        DateTime shiftFrom = new(2022, 10, 3, 9, 0, 0);
        DateTime shiftTo = shiftFrom.AddMinutes(serviceDuration);
        Employee tom = new(){ Id = "Tom", Name = "Thomas Fringe", Shifts = new []
        {
            new Shift { Starting = shiftFrom, Ending = shiftTo }
        }};
        Service mens30Min = new() { Id = "MensCut30Min", AppointmentTimeSpanInMin = 30, IsActive = true };
        var tomsAppointments = Array.Empty<Appointment>(); 

        _serviceRepository.GetActiveService(Arg.Any<string>())
            .ReturnsForAnyArgs(mens30Min);
        _employeeRepository.GetItemAsync(Arg.Any<string>())
            .ReturnsForAnyArgs(tom);
        _appointmentRepository.GetAppointmentsByEmployeeIdAsync(Arg.Is("Tom"))
            .Returns(Task.FromResult((IEnumerable<Appointment>) tomsAppointments));

        _sut = new SlotsService(_serviceRepository, _employeeRepository, _appointmentRepository, _nowService,
            _settings);
        
        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mens30Min.Id, tom.Id);
        
        // Assert
        var slotTimes = slots.DaysSlots.SelectMany(s => s.Times).ToArray();
        Assert.Equal(totalSlots, slotTimes.Length);
        for (int idx = 0; idx < expectedTimes.Length; idx++)
        {
            Assert.Equal(DateTime.Parse(expectedTimes[idx]), slotTimes[idx]);
        }
    }
}