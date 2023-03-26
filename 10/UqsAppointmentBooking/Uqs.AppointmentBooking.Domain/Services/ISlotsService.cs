using Uqs.AppointmentBooking.Domain.Report;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface ISlotsService
{
    Task<Slots> GetAvailableSlotsForEmployee(string serviceId, string employeeId);
}