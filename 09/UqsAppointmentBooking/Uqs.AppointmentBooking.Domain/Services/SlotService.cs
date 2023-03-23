using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.Database;
using Uqs.AppointmentBooking.Domain.Report;

namespace Uqs.AppointmentBooking.Domain.Services;

public class SlotService
{
    private readonly ApplicationContext _context;
    private readonly INowService _nowService;
    private readonly ApplicationSettings _settings;

    public SlotService(ApplicationContext context, INowService nowService, IOptions<ApplicationSettings> settings)
    {
        _context = context;
        _nowService = nowService;
        _settings = settings.Value;
    }

    public Task<Slots> GetAvailableSlotsForEmployee(int serviceId)
    {
        throw new NotImplementedException();
    }
}