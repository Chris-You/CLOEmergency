using CLOEmergency.Models;

namespace CLOEmergency.Services
{
    public interface IEmployeeService
    {
        public List<EmployeeDTO> GetEmployeeList();

        public EmployeeDTO GetEmployee(string name);

        public List<EmployeeDTO> InsertEmployee(List<Employee> employeeList);
    }
}
