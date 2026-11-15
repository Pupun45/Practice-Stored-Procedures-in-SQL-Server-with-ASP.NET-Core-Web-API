
namespace StudentAPI.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Course { get; set; } = string.Empty;
    }
}
