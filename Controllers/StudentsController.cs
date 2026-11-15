
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StudentsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // DATABASE CONNECTION STRING
        private string ConnectionString =>
            _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Database connection string not found."
            );


        // ==========================================
        // 1. GET ALL STUDENTS
        // GET: api/Students
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = new List<Student>();

            try
            {
                using (SqlConnection con =
                    new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd =
                        new SqlCommand("GetAllStudents", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        await con.OpenAsync();

                        using (SqlDataReader reader =
                            await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                students.Add(new Student
                                {
                                    StudentId = Convert.ToInt32(
                                        reader["StudentId"]),

                                    Name = reader["Name"].ToString()!,

                                    Age = Convert.ToInt32(
                                        reader["Age"]),

                                    Course = reader["Course"].ToString()!
                                });
                            }
                        }
                    }
                }

                return Ok(students);
            }
            catch (SqlException ex)
            {
                return StatusCode(
                    500,
                    new { message = "Database error.", error = ex.Message }
                );
            }
        }


        // ==========================================
        // 2. GET STUDENTS BY COURSE
        // GET: api/Students/by-course/MCA
        // ==========================================

        [HttpGet("by-course/{course}")]
        public async Task<IActionResult> GetStudentsByCourse(
            string course)
        {
            if (string.IsNullOrWhiteSpace(course))
            {
                return BadRequest("Course is required.");
            }

            var students = new List<Student>();

            try
            {
                using (SqlConnection con =
                    new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd =
                        new SqlCommand("GetStudentsByCourse", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(
                            "@Course",
                            SqlDbType.VarChar,
                            50
                        ).Value = course;

                        await con.OpenAsync();

                        using (SqlDataReader reader =
                            await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                students.Add(new Student
                                {
                                    StudentId = Convert.ToInt32(
                                        reader["StudentId"]),

                                    Name = reader["Name"].ToString()!,

                                    Age = Convert.ToInt32(
                                        reader["Age"]),

                                    Course = reader["Course"].ToString()!
                                });
                            }
                        }
                    }
                }

                return Ok(students);
            }
            catch (SqlException ex)
            {
                return StatusCode(
                    500,
                    new { message = "Database error.", error = ex.Message }
                );
            }
        }


        // ==========================================
        // 3. INSERT NEW STUDENT
        // POST: api/Students
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> InsertStudent(
            [FromBody] Student student)
        {
            if (student == null)
            {
                return BadRequest("Student details are required.");
            }

            if (string.IsNullOrWhiteSpace(student.Name) ||
                string.IsNullOrWhiteSpace(student.Course) ||
                student.Age <= 0)
            {
                return BadRequest(
                    "Please provide valid student details."
                );
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd =
                        new SqlCommand("InsertStudent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(
                            "@Name",
                            SqlDbType.VarChar,
                            100
                        ).Value = student.Name;

                        cmd.Parameters.Add(
                            "@Age",
                            SqlDbType.Int
                        ).Value = student.Age;

                        cmd.Parameters.Add(
                            "@Course",
                            SqlDbType.VarChar,
                            50
                        ).Value = student.Course;

                        await con.OpenAsync();

                        object? result =
                            await cmd.ExecuteScalarAsync();

                        if (result == null || result == DBNull.Value)
                        {
                            return StatusCode(
                                500,
                                "Student was inserted but no ID was returned."
                            );
                        }

                        student.StudentId = Convert.ToInt32(result);
                    }
                }

                return StatusCode(
                    StatusCodes.Status201Created,
                    new
                    {
                        message = "Student inserted successfully.",
                        data = student
                    }
                );
            }
            catch (SqlException ex)
            {
                return StatusCode(
                    500,
                    new { message = "Database error.", error = ex.Message }
                );
            }
        }


        // ==========================================
        // 4. UPDATE STUDENT
        // PUT: api/Students/1
        // ==========================================

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(
            int id,
            [FromBody] Student student)
        {
            if (student == null)
            {
                return BadRequest("Student details are required.");
            }

            if (id <= 0 || id != student.StudentId)
            {
                return BadRequest("Student ID does not match.");
            }

            if (string.IsNullOrWhiteSpace(student.Name) ||
                string.IsNullOrWhiteSpace(student.Course) ||
                student.Age <= 0)
            {
                return BadRequest(
                    "Please provide valid student details."
                );
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd =
                        new SqlCommand("UpdateStudent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(
                            "@StudentId",
                            SqlDbType.Int
                        ).Value = id;

                        cmd.Parameters.Add(
                            "@Name",
                            SqlDbType.VarChar,
                            100
                        ).Value = student.Name;

                        cmd.Parameters.Add(
                            "@Age",
                            SqlDbType.Int
                        ).Value = student.Age;

                        cmd.Parameters.Add(
                            "@Course",
                            SqlDbType.VarChar,
                            50
                        ).Value = student.Course;

                        await con.OpenAsync();

                        object? result =
                            await cmd.ExecuteScalarAsync();

                        int rowsAffected =
                            result == null || result == DBNull.Value
                                ? 0
                                : Convert.ToInt32(result);

                        if (rowsAffected == 0)
                        {
                            return NotFound("Student not found.");
                        }
                    }
                }

                return Ok(new
                {
                    message = "Student updated successfully."
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(
                    500,
                    new { message = "Database error.", error = ex.Message }
                );
            }
        }


        // ==========================================
        // 5. DELETE STUDENT
        // DELETE: api/Students/1
        // ==========================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Student ID.");
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd =
                        new SqlCommand("DeleteStudent", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add(
                            "@StudentId",
                            SqlDbType.Int
                        ).Value = id;

                        await con.OpenAsync();

                        object? result =
                            await cmd.ExecuteScalarAsync();

                        int rowsAffected =
                            result == null || result == DBNull.Value
                                ? 0
                                : Convert.ToInt32(result);

                        if (rowsAffected == 0)
                        {
                            return NotFound("Student not found.");
                        }
                    }
                }

                return Ok(new
                {
                    message = "Student deleted successfully."
                });
            }
            catch (SqlException ex)
            {
                return StatusCode(
                    500,
                    new { message = "Database error.", error = ex.Message }
                );
            }
        }
    }
}
