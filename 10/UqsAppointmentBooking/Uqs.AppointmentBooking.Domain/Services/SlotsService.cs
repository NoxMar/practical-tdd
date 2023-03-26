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

        return null!;
    }
}