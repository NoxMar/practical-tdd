using Microsoft.Extensions.Logging.Abstractions;
using Uqs.Weather.Controllers;

NullLogger<WeatherForecastController> logger = new();
WeatherForecastController controller = new(logger, null!);

var f1 = controller.ConvertCToF(-1);
// ReSharper disable once CompareOfFloatsByEqualityOperator
if (f1 != 30.20d)
{
    throw new Exception("Invalid");
}
var f2 = controller.ConvertCToF(1.2);
// ReSharper disable once CompareOfFloatsByEqualityOperator
if (f2 != 34.16d)
{
    throw new Exception("Invalid");
}
Console.WriteLine("Test Passed");