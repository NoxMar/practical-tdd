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
    private readonly IOptions<ApplicationSettings> _settings = Substitute.For<IOptions<ApplicationSettings>>();

    [Fact(Skip = "TODO")]
    public async Task GetAvailableSlotsForEmployee_ServiceIdNotFound_ArgumentException()
    {
        // Arrange
        var ctx = _ctxBuilder
            .Build();
        _sut = new(ctx, _nowService, _settings);
        
        // Act
        var e = await Record.ExceptionAsync(() => _sut.GetAvailableSlotsForEmployee(-1));
        
        // Assert
        Assert.IsType<ArgumentException>(e);
    }
}