using AdamTibi.OpenWeather;

namespace Uqs.Weather.Tests.Unit.Stubs;

public class ClientStub : IClient
{
    private readonly DateTime _now;
    private readonly IEnumerable<double> _sevenDayTemps;

    public ClientStub(DateTime now, IEnumerable<double> sevenDayTemps)
    {
        _now = now;
        _sevenDayTemps = sevenDayTemps;
    }

    public Task<OneCallResponse> OneCallAsync(decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit)
    {
        const int days = 7;
        OneCallResponse res = new();
        res.Daily = new Daily[days];
        for (int i = 0; i < days; i++)
        {
            res.Daily[i] = new()
            {
                Dt = _now.AddDays(i),
                Temp = new (){ Day = _sevenDayTemps.ElementAt(i) }
            };
        }

        return Task.FromResult(res);
    }
}