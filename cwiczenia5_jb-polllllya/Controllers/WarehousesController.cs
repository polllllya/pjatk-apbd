using System.Data;
using Microsoft.AspNetCore.Mvc;
using Zadanie5.DTOs;
using Zadanie5.Services;
using System.Data.SqlClient;

namespace Zadanie5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
    
        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }
    
        [HttpPost]
        public ActionResult AddProduct(ProductDTO product)
        {
            var result = _warehouseService.AddProduct(product);
            return result.Item1 switch
            {
                400 => BadRequest(result.Item2),
                404 => NotFound(result.Item2),
                200 => Ok(result.Item2)
            };
        }
    }
    
    [Route("api/[controller]")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public Warehouses2Controller(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost]
        public IActionResult AddProduct(ProductDTO product)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=s24601;User Id=sa;Password=Test123@"))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("master.dbo.AddProductToWarehouse", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdProduct", product.IdProduct);
                    command.Parameters.AddWithValue("@IdWarehouse", product.IdWarehouse);
                    command.Parameters.AddWithValue("@Amount", product.Amount);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    try
                    {
                        command.ExecuteNonQuery();
                        return Ok("Product added to warehouse successfully.");
                    }
                    catch (SqlException ex)
                    { 
                        return BadRequest(ex.Message);
                    }
                }
            }
        }
    }
}