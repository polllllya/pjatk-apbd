using Microsoft.AspNetCore.Mvc;

namespace Zadanie3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly string _csvPath = "/Users/polya.bragina/Desktop/APBD/cwiczenia3_jb-polllllya/Data/data.csv";

        // GET /students
        [HttpGet]
        public ActionResult Get()
        {
            var students = ReadStudentsFromFile();
            return Ok(students);
        }

        // GET /students/s1234
        [HttpGet("{id}")]
        public ActionResult GetById(string id)
        {
            var student = ReadStudentsFromFile().FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound("There is no student with this index");
            }

            return Ok(student);
        }

        // PUT /students/s1234
        [HttpPut("{id}")]
        public ActionResult UpdateStudent(string id, Student updatedStudent)
        {
            var students = ReadStudentsFromFile();
            var existingStudent = students.FirstOrDefault(s => s.Id == id);
            if (existingStudent == null)
            {
                return NotFound("There is no student with this index");
            }
            
            if (existingStudent.Id != updatedStudent.Id)
            {
                return BadRequest("You can't change the index");
            }
            
            // Copy updated fields to existing student object without Id
            existingStudent.FirstName = updatedStudent.FirstName;
            existingStudent.LastName = updatedStudent.LastName;
            existingStudent.Specialization = updatedStudent.Specialization;            
            existingStudent.StudyMode = updatedStudent.StudyMode;
            existingStudent.Email = updatedStudent.Email;
            existingStudent.DateOfBirth = updatedStudent.DateOfBirth;
            existingStudent.MotherFirstName = updatedStudent.MotherFirstName;
            existingStudent.FatherFirstName = updatedStudent.FatherFirstName;
            
            WriteStudentsToFile(students);
            return Ok(existingStudent);
        }

        // POST /students
        [HttpPost]
        public ActionResult AddStudent(Student student)
        {
            if (NotEnoughData(student))
            {
                return BadRequest("Not enough data entered");
            }

            var students = ReadStudentsFromFile();

            if (SIsMissing(student))
            {
                return BadRequest("The letter 's' at the beginning of the index is missing");
            }

            if (NotUniqueIndex(students, student))
            {
                return BadRequest("Such an index already exists");
            }

            students.Add(student);
            WriteStudentsToFile(students);

            return Ok();
        }

        // DELETE / students/s1234
        [HttpDelete("{id}")]
        public ActionResult DeleteStudent(string id)
        {
            var student = ReadStudentsFromFile().FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound("There is no student with this index");
            }

            var students = ReadStudentsFromFile();

            students.RemoveAll(s => s.Id == id);

            WriteStudentsToFile(students);

            return Ok("This student has been deleted");
        }

        private static bool NotEnoughData(Student student)
        {
            return student.FirstName == "" || student.LastName == "" || student.Specialization == "" || student.StudyMode == ""
                   || student.DateOfBirth == "" || student.Email == "" || student.Id == "" ||
                   student.FatherFirstName == "" ||
                   student.MotherFirstName == "";
        }

        private static bool SIsMissing(Student student)
        {
            return student.Id[0] != 's';
        }

        private static bool NotUniqueIndex(List<Student> students, Student student)
        {
            return students.Exists(el => el.Id == student.Id);
        }

        private List<Student> ReadStudentsFromFile()
        {
            if (!System.IO.File.Exists(_csvPath))
            {
                return new List<Student>();
            }

            var lines = System.IO.File.ReadAllLines(_csvPath);
            var students = new List<Student>();
            for (var line = 1; line < lines.Length; line++)
            {
                var parts = lines[line].Split(',');

                var student = new Student
                (
                    parts[0],
                    parts[1],
                    parts[2],
                    parts[3],
                    parts[4],
                    parts[5],
                    parts[6],
                    parts[7],
                    parts[8]
                );
                students.Add(student);
            }

            return students;
        }

        private void WriteStudentsToFile(List<Student> students)
        {
            var lines = new List<string>();
            lines.Add("Name,Surname,StudiesName,StudiesMode,Index,BirthDate,Email,MothersName,FathersName");
            foreach (var student in students)
            {
                var line = string.Join(",", student.FirstName, student.LastName, student.Specialization, student.StudyMode,
                    student.Id, student.DateOfBirth, student.Email, student.MotherFirstName, student.FatherFirstName);
                lines.Add(line);
            }

            System.IO.File.WriteAllLines(_csvPath, lines);
        }
    }
}