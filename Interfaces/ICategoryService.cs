using E_Commerce.DTO;

namespace E_Commerce.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    }
}
