using Microsoft.Extensions.Logging.Abstractions;
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
        const double nextDayTemp = 3.3;
        var today = new DateTime(2022, 1, 1);
        var readWeatherTemps = new[] { 2, nextDayTemp, 4, 5.5, 6, 7.7, 8 };
        Stubs.ClientStub clientStub = new(today, readWeatherTemps);
        WeatherForecastController controller = new(null!, clientStub, null!, null!);
        
        // Act
        IEnumerable<WeatherForecast> sut = await controller.GetReal();
        
        // Assert
        Assert.Equal(3, sut.First().TemperatureC);
    }
}