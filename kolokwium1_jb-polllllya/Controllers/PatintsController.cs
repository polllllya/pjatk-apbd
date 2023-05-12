using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Kol1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly string _connectionString =
            "Data Source=localhost;Initial Catalog=s24601;User Id=sa;Password=Test123@";

        [HttpGet]
        public IActionResult GetPatients(string lastName)
        {
            List<Patient> patients = new List<Patient>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(
                    "SELECT * FROM master.dbo.Patient WHERE LastName = @LastName",
                    connection);
                command.Parameters.AddWithValue("@LastName", lastName);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Patient patient = new Patient
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        LastName = reader.GetString(2),
                        DateOfBirth = reader.GetDateTime(3)
                    };
                    patients.Add(patient);
                }
            }

            if (patients.Count > 0)
            {
                return Ok(patients);
            }
            else
            {
                return NotFound("There is no patient with the given lastname");
            }
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly string _connectionString =
            "Data Source=localhost;Initial Catalog=s24601;User Id=sa;Password=Test123@";

        [HttpPost]
        public IActionResult AddPrescription(Prescription prescription)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Sprawdzamy czy amountjest większe od 0
                        if (prescription.Amount <= 0)
                        {
                            return BadRequest("Amount must be greater than 0");
                        }


                        // Sprawdzenie, czy pacjent o podanym Id istnieje w bazie
                        using (SqlCommand command = new SqlCommand(
                                   "SELECT 1 FROM master.dbo.Patient WHERE Id = @patientId",
                                   connection, transaction))
                        {
                            command.Parameters.AddWithValue("@patientId", prescription.PatientId);

                            object result = command.ExecuteScalar();
                            if (result == null)
                            {
                                return NotFound($"Patient with Id {prescription.PatientId} does not exist.");
                            }
                        }

                        // Sprawdzenie, czy doktor o podanym Id istnieje w bazie
                        using (SqlCommand command = new SqlCommand(
                                   "SELECT 1 FROM master.dbo.Doctor WHERE Id = @doctorId",
                                   connection, transaction))
                        {
                            command.Parameters.AddWithValue("@doctorId", prescription.DoctorId);

                            object result = command.ExecuteScalar();
                            if (result == null)
                            {
                                return NotFound($"Doctor with Id {prescription.DoctorId} does not exist.");
                            }
                        }

                        // Sprawdzamy czy lek o podanej nazwie istnieje.
                        using (SqlCommand command = new SqlCommand(
                                   "SELECT 1 FROM master.dbo.Medicine WHERE Name = @medicineName",
                                   connection, transaction))
                        {
                            command.Parameters.AddWithValue("@medicineName", prescription.Medicine);

                            object result = command.ExecuteScalar();
                            if (result == null)
                            {
                                //dodajemy go do bazy i pobieramy jego id
                                using (SqlCommand newCommand = new SqlCommand(
                                           "INSERT INTO master.dbo.Medicine (Name) VALUES (@medicineName)", connection,
                                           transaction))
                                {
                                    newCommand.Parameters.AddWithValue("@medicineName", prescription.Medicine);
                                    newCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        int idM;
                        using (SqlCommand command = new SqlCommand(
                                   "SELECT TOP 1 Id FROM master.dbo.Medicine WHERE Name = @medicineName", connection,
                                   transaction))
                        {
                            command.Parameters.AddWithValue("@medicineName", prescription.Medicine);
                            idM = (int)command.ExecuteScalar();
                            command.ExecuteNonQuery();
                        }


                        // Dodajemy nową receptę
                        using (SqlCommand command = new SqlCommand(
                                   "INSERT INTO master.dbo.Prescription (Doctor_Id, Patient_Id, Medicine_Id, Amount, CreatedAt) VALUES (@doctorId, @patientId, @idM, @amount, GETDATE())",
                                   connection, transaction))
                        {
                            command.Parameters.AddWithValue("@doctorId", prescription.DoctorId);
                            command.Parameters.AddWithValue("@patientId", prescription.PatientId);
                            command.Parameters.AddWithValue("@medicineName", prescription.Medicine);
                            command.Parameters.AddWithValue("@amount", prescription.Amount);
                            command.Parameters.AddWithValue("idM", idM);



                            command.ExecuteNonQuery();
                        }

                        int idP;
                        using (SqlCommand command = new SqlCommand(
                                   "SELECT Id FROM master.dbo.Prescription WHERE Doctor_Id = @doctorId AND Patient_Id = @patientId AND Medicine_Id = (SELECT Id FROM master.dbo.Medicine WHERE Name = @medicineName)",
                                   connection, transaction))
                        {
                            command.Parameters.AddWithValue("@patientId", prescription.PatientId);
                            command.Parameters.AddWithValue("@doctorId", prescription.DoctorId);
                            command.Parameters.AddWithValue("@medicineName", prescription.Medicine);

                            idP = (int)command.ExecuteScalar();
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        return Ok("A new Prescription has been added. Id of new Prescription is " + idP);
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, ex.Message);
                    }
                }
            }
        }
    }
}