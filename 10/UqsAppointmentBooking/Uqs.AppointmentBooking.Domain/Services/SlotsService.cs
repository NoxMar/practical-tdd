using Microsoft.Extensions.Options;
using Uqs.AppointmentBooking.Domain.Report;
using Uqs.AppointmentBooking.Domain.Repositories;

namespace Uqs.AppointmentBooking.Domain.Services;

public class SlotsService : ISlotsService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ApplicationSettings _applicationSettings;
    private readonly DateTime _now;
    public SlotsService(IServiceRepository serviceRepository, IEmployeeRepository employeeRepository,
        IAppointmentRepository appointmentRepository, INowService nowService, IOptions<ApplicationSettings> settings)
    {
        _serviceRepository = serviceRepository;
        _employeeRepository = employeeRepository;
        _appointmentRepository = appointmentRepository;
        _now = nowService.Now;
        _applicationSettings = settings.Value;
    }

    public async Task<Slots> GetAvailableSlotsForEmployee(string serviceId, string employeeId)
    {
        var service = await _serviceRepository.GetActiveService(serviceId);
        if (service is null)
        {
            throw new ArgumentException("Record not found", nameof(serviceId));
        }

        var employee = await _employeeRepository.GetItemAsync(employeeId);
        if (employee is null)
        {
            throw new ArgumentException("Record not found", nameof(employeeId));
        }

        var shifts = employee.Shifts!
            .Where(s => (s.Starting <= _now && s.Ending > _now) || s.Starting > _now)
            .ToArray();

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
                
                possibleAppointmentStart =  possibleAppointmentStart.AddMinutes(_applicationSettings.RoundUpInMin);
                possibleAppointmentEnd = possibleAppointmentEnd.AddMinutes(_applicationSettings.RoundUpInMin);
            }
        }

        var uniqueDays = possibleAppointmentStarts
            .Select(ti => ti.Date)
            .Distinct();

        var daySlots = possibleAppointmentStarts
            .GroupBy(ds => ds.Date)
            .Select(g => new DaySlots(g.Key, g.ToArray()))
            .ToArray();
        
        return new Slots(daySlots);
    }
}