using System.Collections;
using Uqs.AppointmentBooking.Domain.Repositories;

namespace Uqs.AppointmentBooking.Domain.Services;

public class ServicesService
{
    private readonly IServiceRepository _serviceRepository;
    public ServicesService(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable> GetActiveServices() 
        => await _serviceRepository.GetActiveServices();
}