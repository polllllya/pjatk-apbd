using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Zadanie4.DTOs;

namespace Zadanie4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get([FromQuery] string orderBy = "Name")
        {
            using (SqlConnection connection =
                   new SqlConnection("Data Source=localhost;Initial Catalog=s24601;User Id=sa;Password=Test123@"))
            {
                connection.Open();

                var column = "Name";
                switch (orderBy)
                {
                    case "description":
                        column = "description";
                        break;
                    case "category":
                        column = "category";
                        break;
                    case "area":
                        column = "area";
                        break;
                }

                using SqlCommand command = new SqlCommand($"SELECT * FROM Animal ORDER BY {column} ASC", connection);

                SqlDataReader reader = command.ExecuteReader();

                List<AnimalDTO> animals = new List<AnimalDTO>();

                while (reader.Read())
                {
                    AnimalDTO animal = new AnimalDTO
                    {
                        IdAnimal = (int)reader["IdAnimal"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"],
                        Category = (string)reader["Category"],
                        Area = (string)reader["Area"]
                    };

                    animals.Add(animal);
                }

                reader.Close();
                connection.Close();

                return Ok(animals);
            }
        }

        [HttpPost]
        public ActionResult Post(AnimalDTO animal)
        {
            using (SqlConnection connection =
                   new SqlConnection("Data Source=localhost;Initial Catalog=s24601;User Id=sa;Password=Test123@"))
            {
                connection.Open();

                using SqlCommand command =
                    new SqlCommand(
                        "INSERT INTO Animal (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)",
                        connection);

                command.Parameters.AddWithValue("@Name", animal.Name);
                command.Parameters.AddWithValue("@Description", animal.Description);
                command.Parameters.AddWithValue("@Category", animal.Category);
                command.Parameters.AddWithValue("@Area", animal.Area);
                
                connection.Close();

                return Ok();
            }
        }
        
        [HttpPut]
        [Route("api/animals/{idAnimal}")]
        public IActionResult UpdateAnimal(int idAnimal, [FromBody] AnimalDTO animal)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=s24601;User Id=sa;Password=Test123@"))
            {
                connection.Open();

                using SqlCommand command = new SqlCommand("UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal", connection);

                command.Parameters.AddWithValue("@Name", animal.Name);
                command.Parameters.AddWithValue("@Description", animal.Description);
                command.Parameters.AddWithValue("@Category", animal.Category);
                command.Parameters.AddWithValue("@Area", animal.Area);
                    
                
                var numberOfRows = (int?)command.ExecuteNonQuery();
                connection.Close();

                if (numberOfRows == 0)
                {
                    return NotFound();
                }

                return Ok();
            }
        }


        [HttpDelete]
        [Route("api/animals/{idAnimal}")]
        public IActionResult DeleteAnimal(int idAnimal)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=s24601;User Id=sa;Password=Test123@"))
            {
                connection.Open();

                using SqlCommand command = new SqlCommand("DELETE FROM Animal WHERE IdAnimal = @IdAnimal", connection);

                command.Parameters.AddWithValue("@IdAnimal", idAnimal);

                var numberOfRows = (int?)command.ExecuteNonQuery();
                connection.Close();

                if (numberOfRows == 0)
                {
                    return NotFound();
                }

                return Ok();
            }
        }

    }
}