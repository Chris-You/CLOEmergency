using CLOEmergency.Models;
using AutoMapper;
namespace CLOEmergency.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        // 테스트용 데이터 저장소 
        public List<EmployeeDTO> _employees;


        public EmployeeRepository()
        {
            _employees = new List<EmployeeDTO>();
            _employees.Add(new EmployeeDTO { Name = "김철수", Email = "chulsu@naver.com", Tel = "010-1111-2222", Joined = "2022-11-10" });
            _employees.Add(new EmployeeDTO { Name = "박영희", Email = "younghee@naver.com", Tel = "010-1234-5678", Joined = "2022-11-11" });
            _employees.Add(new EmployeeDTO { Name = "홍길동", Email = "chulsu@naver.com", Tel = "010-4455-7890", Joined = "2022-11-12" } );
        }


        public List<EmployeeDTO> GetEmployeeList() => _employees;
        

        public EmployeeDTO GetEmployee(string name)
        {
            var emp = _employees.Where(emp => emp.Name == name);

            if (emp.Count() > 0)
                return emp.First();
            else
                return null;
        }

        public EmployeeDTO InsEmployee(EmployeeDTO employee)
        {
            _employees.Add(employee);

            return employee;
        }

    }
}
