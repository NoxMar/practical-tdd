using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Report;
using Uqs.AppointmentBooking.Domain.Repositories;

namespace Uqs.AppointmentBooking.Domain.Services;
public class ServicesService : IServicesService
{
    private readonly IServiceRepository _serviceRepository;
    public ServicesService(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Service?> GetService(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Service>> GetActiveServices() 
        => await _serviceRepository.GetActiveServices();
}