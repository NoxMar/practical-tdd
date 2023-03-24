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

        var now = _nowService.Now;
        var shifts = _context.Shifts!
            .Where(x => x.EmployeeId == employeeId)
            .Where(s => (s.Starting <= now && s.Ending > now) || s.Starting > now);
        if (!shifts.Any())
        {
            return new Slots(Array.Empty<DaySlots>());
        }

        List<DateTime> possibleAppointmentStarts = new();

        foreach (var shift in shifts)
        {
            var possibleAppointmentStart = shift.Starting;
            var possibleAppointmentEnd = possibleAppointmentStart.AddMinutes(service.AppointmentTimeSpanInMin);

            while (possibleAppointmentEnd <= shift.Ending)
            {
                possibleAppointmentStarts.Add(possibleAppointmentStart);
                
                possibleAppointmentStart =  possibleAppointmentStart.AddMinutes(_settings.RoundUpInMin);
                possibleAppointmentEnd = possibleAppointmentEnd.AddMinutes(_settings.RoundUpInMin);
            }
        }

        var uniqueDays = possibleAppointmentStarts.Select(ti => ti.Date).Distinct();

        var daySlotList = new List<DaySlots>();

        foreach (var day in uniqueDays)
        {
            var startTimes = possibleAppointmentStarts
                .Where(x => x.Date == day.Date)
                .ToArray();
            daySlotList.Add(new(day, startTimes));
        }

        Slots slots = new(daySlotList.ToArray());

        return slots;
    }
}