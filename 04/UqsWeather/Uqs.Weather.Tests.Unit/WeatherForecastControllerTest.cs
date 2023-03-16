using AdamTibi.OpenWeather;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Uqs.Weather.Controllers;

namespace Uqs.Weather.Tests.Unit;

public class WeatherForecastControllerTest
{
    [Theory]
    [InlineData(0    , 32)]
    [InlineData(-100 , -148)]
    [InlineData(-10.1, 13.8)]
    [InlineData(10   , 50)]
    public void ConvertCToF_Cel_CorrectFah(double c, double f)
    {
        // Arrange
        var logger = NullLogger<WeatherForecastController>.Instance;
        WeatherForecastController controller = new(logger, null!, null!, null!);
        // Act
        double actual = controller.ConvertCToF(c);
        // Assert
        Assert.Equal(f, actual, 1);
    }

    [Fact]
    public async Task GetReal_NotInterestedInTodayWeather_WFStartsFromNextDay()
    {
        // Arrange
        var today = new DateTime(2022, 1, 1);
        var readWeatherTemps = new[] { 2, 3.3, 4, 5.5, 6, 7.7, 8 };
        var clientMock = Substitute.For<IClient>();
        clientMock.OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(),
                Arg.Any<IEnumerable<Excludes>>(), Arg.Any<Units>())
            .Returns(_ =>
            {
                const int days = 7;
                OneCallResponse res = new();
                res.Daily = new Daily[days];
                for (int i = 0; i < days; i++)
                {
                    res.Daily[i] = new()
                    {
                        Dt = today.AddDays(i),
                        Temp = new() { Day = readWeatherTemps.ElementAt(i) }
                    };
                }

                return Task.FromResult(res);
            });
        WeatherForecastController sut = new(null!, clientMock, null!, null!);
        
        // Act
        IEnumerable<WeatherForecast> wfs = await sut.GetReal();
        
        // Assert
        Assert.Equal(3, wfs.First().TemperatureC);
    }

    [Fact]
    public async Task GetReal_5DaysForecastStartingNextDay_WF5ThDayIsRealWeather6ThDay()
    {
        // Arrange
        var realWeatherTemps = new[] { 2, 3, 4, 5, 6.6, 7.7, 8 };
        Stubs.ClientStub clientStub = new(DateTime.Today, realWeatherTemps);
        WeatherForecastController sut = new(null!, clientStub, null!, null!);
        
        // Act
        IEnumerable<WeatherForecast> wfs = await sut.GetReal();
        
        // Assert
        Assert.Equal(8, wfs.Last().TemperatureC);
    }

    [Fact]
    public async Task GetReal_ForecastingFor5DaysOnly_WFHas5Days()
    {
        // Arrange
        var realWeatherTemps = new[] { 2, 3, 4, 5, 6.6, 7, 8.8 };
        Stubs.ClientStub clientStub = new(DateTime.Today, realWeatherTemps);
        WeatherForecastController sut = new(null!, clientStub, null!, null!);

        // Act
        IEnumerable<WeatherForecast> wfs = await sut.GetReal();
        
        // Assert
        Assert.Equal(5, wfs.Count());
    }
    
    [Theory]
    [InlineData(2.2 , 2)]
    [InlineData(2.5 , 2)]
    [InlineData(2.51, 3)]
    [InlineData(2.8 , 3)]
    public async Task GetReal_WFDoesntConsiderDecimal_RealWeatherTempRoundedProperly(double realTemp, int expected)
    {
        // Arrange
        var realWeatherTemps = new[] { 1, realTemp, 3, 4, 5, 6, 7.7 };
        Stubs.ClientStub clientStub = new(DateTime.Today, realWeatherTemps);
        WeatherForecastController sut = new(null!, clientStub, null!, null!);
        
        // Act
        IEnumerable<WeatherForecast> wfs = await sut.GetReal();
        
        // Assert
        Assert.Equal(expected, wfs.First().TemperatureC);
    }

    [Fact]
    public async Task GetReal_TodayWeatherAnd6DaysForecastReceived_RealDateMatchesNextDay()
    {
        // Arrange
        var realWeatherTemps = new[] { 1, 2, 3, 4, 5, 6, 7.7 };
        DateTime today = new(2022, 1, 1);
        Stubs.ClientStub clientStub = new(today, realWeatherTemps);
        WeatherForecastController sut = new(null!, clientStub, null!, null!);
        
        // Act
        IEnumerable<WeatherForecast> wfs = await sut.GetReal();
        
        // Assert
        Assert.Equal(new DateTime(2022, 1, 2), wfs.First().Date);
    }
    
    [Fact]
    public async Task GetReal_TodayWeatherAnd6DaysForecastReceived_RealDateMatchesLastDay()
    {
        // Arrange
        var realWeatherTemps = new[] { 1, 2, 3, 4, 5, 6, 7.7 };
        DateTime today = new(2022, 1, 1);
        Stubs.ClientStub clientStub = new(today, realWeatherTemps);
        WeatherForecastController sut = new(null!, clientStub, null!, null!);
        
        // Act
        IEnumerable<WeatherForecast> wfs = await sut.GetReal();
        
        // Assert
        Assert.Equal(new DateTime(2022, 1, 2), wfs.First().Date);
    }

    [Fact]
    public async Task GetReal_RequestToOpenWeahter_MetricUntiIsUsed()
    {
        // Arrange
        var today = new DateTime(2022, 1, 1);
        var readWeatherTemps = new[] { 2, 3.3, 4, 5.5, 6, 7.7, 8 };
        var clientMock = Substitute.For<IClient>();
        clientMock.OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(),
                Arg.Any<IEnumerable<Excludes>>(), Arg.Any<Units>())
            .Returns(_ =>
            {
                const int days = 7;
                OneCallResponse res = new();
                res.Daily = new Daily[days];
                for (int i = 0; i < days; i++)
                {
                    res.Daily[i] = new()
                    {
                        Dt = today.AddDays(i),
                        Temp = new() { Day = readWeatherTemps.ElementAt(i) }
                    };
                }

                return Task.FromResult(res);
            });
        WeatherForecastController sut = new(null!, clientMock, null!, null!);
        
        // Act
        var _ = await sut.GetReal();
        
        // Assert
        await clientMock.Received().OneCallAsync(Arg.Any<decimal>(), Arg.Any<decimal>(),
            Arg.Any<IEnumerable<Excludes>>(), Arg.Is<Units>(u => u == Units.Metric));
    }
}