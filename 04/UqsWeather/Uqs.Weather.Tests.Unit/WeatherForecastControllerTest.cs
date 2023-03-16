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
}