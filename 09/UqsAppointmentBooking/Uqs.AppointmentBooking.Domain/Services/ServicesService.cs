using Microsoft.EntityFrameworkCore;
using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Database;

namespace Uqs.AppointmentBooking.Domain.Services;

public class ServicesService
{
    private readonly ApplicationContext _context;
    public ServicesService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetActiveServices()
        => await _context.Services!
            .Where(s => s.IsActive)
            .ToArrayAsync();
}