using Uqs.AppointmentBooking.Domain.DomainObjects;
using Uqs.AppointmentBooking.Domain.Repositories;

namespace Uqs.AppointmentBooking.Domain.Services;

public class EmployeesService : IEmployeesService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeesService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        throw new NotImplementedException();
    }
}