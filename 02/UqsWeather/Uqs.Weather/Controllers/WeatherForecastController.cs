using AdamTibi.OpenWeather;
using Microsoft.AspNetCore.Mvc;
using Uqs.Weather.Wrappers;

namespace Uqs.Weather.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private const int FORECAST_DAYS = 5;
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
    private readonly IClient _client;
    private readonly INowWrapper _nowWrapper;
    private readonly IRandomWrapper _randomWrapper;
    public WeatherForecastController( IClient client, ILogger<WeatherForecastController> logger, INowWrapper nowWrapper, IRandomWrapper randomWrapper)
    {
        _client = client;
        _logger = logger;
        _nowWrapper = nowWrapper;
        _randomWrapper = randomWrapper;
    }

    [HttpGet("GetRandomWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index =>
        {
            var temperatureC = _randomWrapper.Next(-20, 55);
            return new WeatherForecast
            {
                Date = _nowWrapper.Now.AddDays(index),
                TemperatureC = temperatureC,
                Summary = MapFeelToTemp(temperatureC)
            };
        }).ToArray();
    }
    
    [HttpGet("GetRealWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetReal()
    {
        const decimal GREENWICH_LAT = 51.4810m;
        const decimal GREENWICH_LON = 0.0052m;
        OneCallResponse res = await _client.OneCallAsync
        (GREENWICH_LAT, GREENWICH_LON, new [] {
            Excludes.Current, Excludes.Minutely,
            Excludes.Hourly, Excludes.Alerts }, Units.Metric);

        WeatherForecast[] wfs = new WeatherForecast[FORECAST_DAYS];
        for (int i = 0; i < wfs.Length; i++)
        {
            var wf = wfs[i] = new();
            wf.Date = res.Daily[i + 1].Dt;
            var forecastedTemp = res.Daily[i + 1].Temp.Day;
            wf.TemperatureC = (int)Math.Round(forecastedTemp);
            wf.Summary = MapFeelToTemp(wf.TemperatureC);
        }
        return wfs;
    }

    [HttpGet("ConvertCToF")]
    public double ConvertCToF(double c)
    {
        var f = c * (9d / 5d) + 32;
        _logger.LogInformation("conversion requested");
        return f;
    }
}