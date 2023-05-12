using Zadanie5.DTOs;

namespace Zadanie5.Services
{
    public interface IWarehouseService
    {
        public Tuple<int,string> AddProduct(ProductDTO product);
    }
}
