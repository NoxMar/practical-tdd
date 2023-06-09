using Microsoft.Extensions.Options;
using NSubstitute;
using Uqs.AppointmentBooking.Domain.Services;
using Uqs.AppointmentBooking.Domain.Tests.Unit.Fakes;

namespace Uqs.AppointmentBooking.Domain.Tests.Unit;

public class SlotsServiceTests
{
    private readonly ApplicationContextFakeBuilder _ctxBuilder = new();
    private SlotService? _sut;
    private readonly INowService _nowService = Substitute.For<INowService>();
    private readonly ApplicationSettings _applicationSettings = new() {RoundUpInMin = 5};
    private readonly IOptions<ApplicationSettings> _settings = Substitute.For<IOptions<ApplicationSettings>>();

    public SlotsServiceTests()
    {
        _settings.Value.Returns(_applicationSettings);
    }
    
    [Fact]
    public async Task GetAvailableSlotsForEmployee_ServiceIdNotFound_ArgumentException()
    {
        // Arrange
        var ctx = _ctxBuilder
            .Build();
        _sut = new(ctx, _nowService, _settings);
        
        // Act
        var e = await Record.ExceptionAsync(() => _sut.GetAvailableSlotsForEmployee(-1, -1));
        
        // Assert
        Assert.IsType<ArgumentException>(e);
    }

    [Fact]
    public async Task GetAvailableSlotsForEmployee_NoShiftsForTomAndNoAppointmentsInSystem_NoSlots()
    {
        // Arrange
        var appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        var ctx = _ctxBuilder
            .WithSingleService(30)
            .WithSingleEmployeeTom()
            .Build();
        _sut = new SlotService(ctx, _nowService, _settings);
        var tom = ctx.Employees!.Single();
        var mensCut30Min = ctx.Services!.Single();
        
        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30Min.Id, tom.Id);
        
        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times);
        Assert.Empty(times);
    }

    [Theory]
    [InlineData(5, 0)]
    [InlineData(25, 0)]
    [InlineData(30, 1, "2022-10-03 09:00:00")]
    [InlineData(35, 2, "2022-10-03 09:00", "2022-10-03 09:05:00")]
    public async Task GetAvailableSlotsForEmployee_OneShiftAndNoExistingAppointments_VaryingSlots(int shiftDuration, int totalSlots, params string[] expectedTimes)
    {
        var appointmentFrom = new DateTime(2022, 10, 3, 7, 0, 0);
        _nowService.Now.Returns(appointmentFrom);
        DateTime shiftFrom = new(2022, 10, 3, 9, 0, 0);
        DateTime shiftTo = shiftFrom.AddMinutes(shiftDuration);
        var ctx = _ctxBuilder
            .WithSingleService(30)
            .WithSingleEmployeeTom()
            .WithSingleShiftForTom(shiftFrom, shiftTo)
            .Build();
        _sut = new SlotService(ctx, _nowService, _settings);
        var tom = ctx.Employees!.Single();
        var mensCut30Min = ctx.Services!.Single();

        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30Min.Id, tom.Id);
        
        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times).ToArray();
        Assert.Equal(totalSlots, times.Length);
        for (int idx = 0; idx < expectedTimes.Length; idx++)
        {
            Assert.Equal(DateTime.Parse(expectedTimes[idx]), times[idx]);
        }
    }

    [Theory(Skip = "TODO")]
    [InlineData("2022-10-03 09:00:00", "2022-10-03 11:10:00", 0)]
    [InlineData("2022-10-03 09:30:00", "2022-10-03 11:10:00", 0)]
    [InlineData("2022-10-03 09:00:00", "2022-10-03 10:45:00", 0)]
    [InlineData("2022-10-03 09:35:00", "2022-10-03 11:10:00", 1, "2022-10-03 09:00:00")]
    [InlineData("2022-10-03 09:40:00", "2022-10-03 11:10:00", 2, "2022-10-03 09:00:00", "2022-10-03 09:05:00")]
    [InlineData("2022-10-03 09:00:00", "2022-10-03 10:30:00", 2, "2022-10-03 10:35:00", "2022-10-03 10:40:00")]
    [InlineData("2022-10-03 09:35:00", "2022-10-03 10:30:00", 3, "2022-10-03 09:00:00", "2022-10-03 10:35:00", "2022-10-03 10:40:00")]
    public async Task GetAvailableSlotsForEmployee_OneShiftWithVaryingAppointments_VaryingSlots(
        string appointmentStartStr, string appointmentEndStr, int totalSlots, params string[] expectedTimes)
    {
        // Arrange
        DateTime appointmentFrom = new(2022, 10, 3, 7, 0, 0); // 7:00
        _nowService.Now.Returns(appointmentFrom);
        DateTime shiftFrom = new(2022, 10, 3, 9, 0, 0); // 9:00
        DateTime shiftTo = new(2022, 10, 3, 11, 10, 0); // 11:10

        DateTime appointmentStart = DateTime.Parse(appointmentStartStr);
        DateTime appointmentEnd = DateTime.Parse(appointmentEndStr);

        var ctx = _ctxBuilder
            .WithSingleService(30)
            .WithSingleEmployeeTom()
            .WithSingleShiftForTom(shiftFrom, shiftTo)
            .WithSingleCustomerPaul()
            .WithSingleAppointmentForTom(appointmentStart, appointmentEnd)
            .Build();
        _sut = new(ctx, _nowService, _settings);
        var tom = ctx.Employees!.Single();
        var mensCut30Min = ctx.Services!.Single();
        
        // Act
        var slots = await _sut.GetAvailableSlotsForEmployee(mensCut30Min.Id, tom.Id);
        
        // Assert
        var times = slots.DaysSlots.SelectMany(x => x.Times).ToArray();
        Assert.Equal(totalSlots, times.Length);
        for (int idx = 0; idx < expectedTimes.Length; idx++)
        {
            Assert.Equal(DateTime.Parse(expectedTimes[idx]), times[idx]);
        }
    }
}