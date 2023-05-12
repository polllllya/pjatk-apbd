using System.ComponentModel.DataAnnotations;

namespace Zadanie5.DTOs
{
    public class ProductDTO
    {
        [Required(ErrorMessage = "IdProduct is required")]
        public int IdProduct { get; set; }
        
        [Required(ErrorMessage = "IdWarehouse is required")]
        public int IdWarehouse { get; set; }
        
        [Range(0, 10, ErrorMessage = "Amount must be in range 0 - 10")]
        public int Amount { get; set; }
        
        [Required(ErrorMessage = "CreatedAt is required")]
        public DateTime CreatedAt { get; set; }
    }
}