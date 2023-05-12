namespace Zadanie3;

public class Student
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialization { get; set; }
    public string StudyMode { get; set; }
    public string Id { get; set; }
    public string DateOfBirth { get; set; }
    public string Email { get; set; }
    public string MotherFirstName { get; set; }
    public string FatherFirstName { get; set; }

    public Student(string firstName, string lastName, string specialization, string studyMode, string id, string dateOfBirth,
        string email, string motherFirstName, string fatherFirstName)
    {
        FirstName = firstName;
        LastName = lastName;
        Specialization = specialization;
        StudyMode = studyMode;
        
        if (id[0] == 's') Id = id; 
        else Id = 's' + id;
        
        DateOfBirth = dateOfBirth;
        Email = email;
        MotherFirstName = motherFirstName;
        FatherFirstName = fatherFirstName;
    }

    public string read()
    {
        return FirstName + " " + LastName + " " + Id;
    }
}