using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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

        /// <summary>
        /// Employee List 
        /// </summary>
        /// <returns></returns>
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
            var employee = new EmployeeDTO();
            try
            {
                employee = _employeeService.GetEmployee(HttpUtility.UrlDecode(name));
                
            }
            catch(Exception e)
            {
                _logger.LogCritical(e, "GetItem Exception");
                employee = null;
            }

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
        /// body(cvs, json) 데이터에 회원정보를 생성
        /// </summary>
        /// <param name="emp">회원정보 형식 데이터</param>
        /// <returns>생성된 회원정보 리턴</returns>
        [HttpPost]
        [Route("body")]
        [Consumes("text/plain")]
        public IActionResult PostBody([FromBody] string body)
        {
            var employeeList = new List<Employee>();
            bool isCsvType = true;
            
            try
            {
                // Request.Body 에 csv, json 이 올지 알수 없기떄문에 string으로 받아서 파싱
                // Deserialize 가 되면 json 형식
                employeeList = JsonConvert.DeserializeObject<List<Employee>>(body);

                isCsvType = false;
            }
            catch (Exception ex) {
                
            }

            // json 형식의 데이터가 아니면 csv 파일을 파싱
            if(isCsvType)
            {
                // 개행문자로 분리
                foreach (string line in body.Split("\r\n"))
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

            
            try
            {
                // 파일 형식이 비정상(데이터가 없음)
                if (employeeList.Count() == 0)
                {
                    _logger.LogWarning(body, "body Warning");
                    return BadRequest("body data is incorrect");
                }
                else
                {
                    var result = _employeeService.InsertEmployee(employeeList);

                    return CreatedAtAction(nameof(GetList), result);
                }
            }
            catch(Exception e)
            {
                _logger.LogCritical(e, "PostBody Exception");
                return NoContent();
            }
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
        /// <returns>생성된 회원정보 리턴</returns>
        [HttpPost]
        public IActionResult Post(IFormFile file)
        {
            try
            {
                var employee = this.ReadFile(file);

                if (employee.Count() == 0)
                {
                    _logger.LogWarning(file.Name, "File Warnning");
                    return BadRequest("file contents are wrong.");
                }
                else
                {
                    //add  service 작업
                    var result = _employeeService.InsertEmployee(employee);

                    return CreatedAtAction(nameof(GetList), result);
                }
            }
            catch(Exception e)
            {
                _logger.LogCritical(e, "PostFile Exception");
                return NoContent();
            }
            
        }


        /// <summary>
        /// 파일을 읽어 처리한다.
        /// </summary>
        /// <param name="file">넘어온 파일 정보</param>
        /// <returns>List<Employee></returns>
        private List<Employee> ReadFile(IFormFile file)
        {
            var employee = new List<Employee>();
            
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                StringBuilder sb = new StringBuilder();
                string jsonEmp = string.Empty;

                while (reader.Peek() >= 0)
                {
                    sb.Append(reader.ReadToEnd());
                }

                if (file.FileName.Split(".").Last().ToLower() == "csv")
                {
                    JArray jArray = new JArray();

                    foreach (var line in sb.ToString().Split("\r\n"))
                    {
                        var emp = line.Split(",");
                        JObject jObject = new JObject();
                        jObject.Add("Name", emp[0]);
                        jObject.Add("Email", emp[1]);
                        jObject.Add("Tel", emp[2]);
                        jObject.Add("Joined", emp[3]);

                        jArray.Add(jObject);
                    }

                    jsonEmp = JsonConvert.SerializeObject(jArray);
                }
                else
                {
                    jsonEmp = sb.ToString();
                }

                employee = JsonConvert.DeserializeObject<List<Employee>>(jsonEmp);
            }

            return employee;
        }
    }
}
