
namespace CLOEmergency.Models
{
    public class Employee
    {
        public Employee()
        {
            Name = string.Empty;
            Email = string.Empty;
            Tel = string.Empty;
            Joined = string.Empty;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("tel")]
        public string Tel { get; set; }

        [JsonProperty("joined")]
        public string Joined { get; set; }
    }

    public class EmployeeDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Tel { get; set; }
        public string? Joined { get; set; }
    }
}
