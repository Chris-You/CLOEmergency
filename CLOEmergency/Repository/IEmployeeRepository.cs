using CLOEmergency.Models;

namespace CLOEmergency.Repository
{
    public interface IEmployeeRepository
    {
        List<EmployeeDTO> GetEmployeeList();
        EmployeeDTO GetEmployee(string name);

        EmployeeDTO InsEmployee(EmployeeDTO employee);
    }
}
