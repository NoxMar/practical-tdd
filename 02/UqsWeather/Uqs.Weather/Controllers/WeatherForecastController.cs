using Microsoft.AspNetCore.Mvc;

namespace Uqs.Weather.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private string MapFeelToTemp(int temperatureC)
    {
        if (temperatureC <= 0)
        {
            return Summaries.First();
        }

        var summariesIndex = temperatureC / 5 + 1;
        return summariesIndex >= Summaries.Length ? Summaries.Last() : Summaries[summariesIndex];
    }
    
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetRandomWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index =>
        {
            var temperatureC = Random.Shared.Next(-20, 55);
            return new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = temperatureC,
                Summary = MapFeelToTemp(temperatureC)
            };
        }).ToArray();
    }
}