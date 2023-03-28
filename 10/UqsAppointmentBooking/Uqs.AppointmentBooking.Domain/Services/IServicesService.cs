using Uqs.AppointmentBooking.Domain.DomainObjects;

namespace Uqs.AppointmentBooking.Domain.Services;

public interface IServicesService
{
    Task<Service?> GetService(string id);

    Task<IEnumerable<Service>> GetActiveServices();
}
