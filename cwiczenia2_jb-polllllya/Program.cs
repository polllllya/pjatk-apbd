using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Zadanie2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var path = args[0];
            var result = await File.ReadAllLinesAsync(path);
            var listOfStudents = new List<Student>();
            string[] oneStudent;

            var sw = new StreamWriter("log.txt"); //znajduje się w bin

            foreach (var student in result)
            {
                oneStudent = student.Split(",");

                if (listOfStudents.Exists(el => el.Index == oneStudent[4])
                    && !listOfStudents.Exists(el =>
                        el.Studies.Exists(st => st.Name == oneStudent[2] && st.Mode == oneStudent[3])))
                {
                    var index = listOfStudents.FindIndex(el => el.Index == oneStudent[4]);
                    listOfStudents[index].Studies.Add(new Studies(oneStudent[2], oneStudent[3]));
                }
                else
                {
                    if (IsThereUnoughData(oneStudent))
                    {
                        listOfStudents.Add(new Student(
                            oneStudent[4],
                            oneStudent[0],
                            oneStudent[1],
                            oneStudent[6],
                            DateOnly.Parse(oneStudent[5]),
                            oneStudent[7],
                            oneStudent[8],
                            new List<Studies> { new Studies(oneStudent[2], oneStudent[3]) }
                        ));
                    }
                    else
                    {
                        try
                        {
                            await sw.WriteLineAsync("Błąd: dla studenta {" + oneStudent[4] + " " + oneStudent[0] + " " +
                                                    oneStudent[1] +
                                                    "} było podano za mało informacji.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception: " + e.Message);
                        }
                    }
                }
            }

            sw.Close();

            foreach (var student in listOfStudents)
            {
                student.AddS();
            }


            var fileName = args[1];
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };

            var jsonString = JsonSerializer.Serialize(
                new { Author = "Palina Brahina", CreatedAt = DateTime.Now, Students = listOfStudents }, options);
            await File.WriteAllTextAsync(fileName, jsonString);
        }

        private static bool IsThereUnoughData(string[] data)
        {
            if (data.Length == 9)
            {
                if (data[0] == "" || data[1] == "" || data[2] == "" || data[3] == ""
                    || data[4] == "" || data[5] == "" || data[6] == "" || data[7] == "" ||
                    data[8] == "")
                    return false;

                return true;
            }

            return false;
        }
        
        
    }


    class Student
    {
        public string Index { get; set; }
        public string Name { get; }
        public string Surname { get; }
        public string Email { get; }
        public DateOnly BirthDate { get; }
        public string MotherName { get; }
        public string FatherName { get; }
        public List<Studies> Studies { get; }

        public Student(string index, string name, string surname, string email, DateOnly birthDate, string motherName,
            string fatherName, List<Studies> studies)
        {
            Index = index;
            Name = name;
            Surname = surname;
            Email = email;
            BirthDate = birthDate;
            MotherName = motherName;
            FatherName = fatherName;
            Studies = studies;
        }

        public void AddS()
        {
            Index = "s" + Index;
        }
    }

    public class Studies
    {
        public string Name { get; }
        public string Mode { get; }

        public Studies(string name, string mode)
        {
            Name = name;
            Mode = mode;
        }
    }
}