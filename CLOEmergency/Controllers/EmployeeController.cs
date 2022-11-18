using AutoMapper;
using CLOEmergency.Models;
using CLOEmergency.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;
using System.Xml.Linq;

namespace CLOEmergency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IConfiguration _config;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, 
                                  IConfiguration config, 
                                  IEmployeeService employeeService)
        {
            _logger = logger;
            _config = config;
            _employeeService = employeeService;
        }


        [HttpGet]
        public IActionResult GetList()
        {
            var employees = _employeeService.GetEmployeeList();
            if (employees.Count() == 0)
            {
                return NoContent();
            }
            else
            {
                return Ok(employees);
            }
        }

        
        [HttpGet("{name}")]
        public IActionResult GetItem(string name)
        {
            var employee = _employeeService.GetEmployee(HttpUtility.UrlDecode(name));

            if (employee == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(employee);
            }
        }

        
        /// <summary>
        ///   body 데이터에 회원정보를 받는다.(json, csv)
        /// </summary>
        /// <param name="emp">회원정보 형식 데이터</param>
        /// <returns></returns>
        [HttpPost]
        [Route("body")]
        [Consumes("text/plain")]
        public IActionResult PostBody([FromBody] string body)
        {
            var employeeList = new List<Employee>();
            bool isJsonType = true;
            // Request.Body 에 어떤 형식이 들어올지 알수 없기떄문에 string으로 받아서 파싱
            try
            {
                employeeList = JsonConvert.DeserializeObject<List<Employee>>(body);
            }
            catch (Exception ex) {
                isJsonType = false;
            }

            // json 형식의 데이터가 아니면 csv 파일을 파싱
            if(!isJsonType)
            {
                // 개행문자로 분리
                foreach (string line in body.Split("\n"))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] emp = line.Split(",");
                        employeeList.Add(new Employee { Name = emp[0], 
                                                    Email = emp[1], 
                                                    Tel = emp[2], 
                                                    Joined = emp[3] });
                    }
                }
            }

            var result = _employeeService.InsertEmployee(employeeList);

            return CreatedAtAction(nameof(GetList), result);
        }

        /*
        /// <summary>
        /// cvs 형식으로  body 전달 
        /// </summary>
        /// <param name="emp">회원정보 cvs 형식</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cvs")]
        [Consumes("text/plain")]
        public IActionResult Post([FromBody] string cvs)
        {
            var employee = new List<Employee>();

            foreach (string line in cvs.Split("\n"))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string[] emp = line.Split(",");
                    employee.Add(new Employee { Name = emp[0], Email = emp[1], Tel = emp[2], Joined = emp[3] });
                }
            }

            

            var result = _employeeService.InsertEmployee(employee);

            return CreatedAtAction(nameof(GetList), result);
        }


        /// <summary>
        /// json 형식으로 body 전달
        /// </summary>
        /// <param name="emp">회원정보 jons 형식</param>
        /// <returns></returns>
        [HttpPost]
        [Route("json")]
        [Consumes("application/json")]
        public IActionResult PostJson([FromBody] List<Employee> json)
        {
            var result = _employeeService.InsertEmployee(json);

            return CreatedAtAction(nameof(GetList), result);
        }
                */

        /// <summary>
        /// 파일을 이용하여 회원등록
        /// </summary>
        /// <param name="file">cvs, json 만 가능</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(IFormFile file)
        {
            var employee = this.ReadFile(file);

            //add  service 작업
            var result =  _employeeService.InsertEmployee(employee);

            return CreatedAtAction(nameof(GetList), result);
        }


        /// <summary>
        /// 파일을 읽어 처리한다.
        /// </summary>
        /// <param name="file">넘어온 파일 정보</param>
        /// <returns></returns>
        private List<Employee> ReadFile(IFormFile file)
        {
            var employee = new List<Employee>();

            if (file.FileName.Split(".").Last().ToLower() == "csv")
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        var emp = reader.ReadLine().Split(",");
                        employee.Add(new Employee { Name = emp[0], Email = emp[1], Tel = emp[2], Joined = emp[3] });
                    }
                }
            }
            else if (file.FileName.Split(".").Last().ToLower() == "json")
            {
                var result = new StringBuilder();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        result.Append(reader.ReadLine());
                    }
                }

                employee = JsonConvert.DeserializeObject<List<Employee>>(result.ToString());
            }

            return employee;


        }

    }
}
