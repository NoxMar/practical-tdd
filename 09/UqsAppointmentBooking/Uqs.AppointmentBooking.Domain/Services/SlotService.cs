using Microsoft.EntityFrameworkCore;
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

    public async Task<Slots> GetAvailableSlotsForEmployee(int serviceId, int employeeId)
    {
        var service = await _context.Services!
            .FirstOrDefaultAsync(s => s.Id == serviceId);
        if (service is null)
        {
            throw new ArgumentException("Record for service not found", nameof(serviceId));
        }

        var shifts = _context.Shifts!.Where(x => x.EmployeeId == employeeId);
        if (!shifts.Any())
        {
            return new Slots(Array.Empty<DaySlots>());
        }
        return null!;
    }
}