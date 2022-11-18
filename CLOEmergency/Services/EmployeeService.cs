using CLOEmergency.Models;
using CLOEmergency.Repository;
using System.Xml.Linq;
using AutoMapper;

namespace CLOEmergency.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IConfiguration _config;
        private readonly IEmployeeRepository _employeeRepository;
        

        public EmployeeService(IConfiguration config, IEmployeeRepository employeeRepository)
        {
            _config = config;
            _employeeRepository = employeeRepository;
        }

        public List<EmployeeDTO> GetEmployeeList()
        {
            return _employeeRepository.GetEmployeeList();
        }

        public EmployeeDTO GetEmployee(string name)
        {
            return _employeeRepository.GetEmployee(name);
        }

        public List<EmployeeDTO> InsertEmployee(List<Employee> employeeList)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Employee, EmployeeDTO>());
            var mapper = new Mapper(config);
            var employeeListDTO = mapper.Map<List<EmployeeDTO>>(employeeList);
            var result = new List<EmployeeDTO>();

            foreach (EmployeeDTO dto in employeeListDTO)
            {
                var emp = _employeeRepository.GetEmployee(dto.Name);
                if (emp == null)
                {
                    result.Add(_employeeRepository.InsEmployee(dto));
                }
            }

            return GetEmployeeList();
        }
    }
}
