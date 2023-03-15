using AdamTibi.OpenWeather;

namespace Uqs.Weather;

public class ClientStub : IClient
{
    public Task<OneCallResponse> OneCallAsync(decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit)
    {
        const int days = 7;
        OneCallResponse res = new();
        res.Daily = new Daily[days];
        DateTime now = DateTime.Now;
        for (int i = 0; i < days; i++)
        {
            res.Daily[i] = new ()
            {
                Dt = now.AddDays(i),
                Temp = new Temp{Day = Random.Shared.Next(-20, 55)}
            };
        }
        return Task.FromResult(res);
    }
}