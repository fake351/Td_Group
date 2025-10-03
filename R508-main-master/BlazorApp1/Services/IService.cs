using BlazorApp.Models;

namespace BlazorApp.Services
{
    public interface IService<TEntity>
    {
        Task<List<TEntity>?> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity updatedEntity);
        Task DeleteAsync(int id);
        Task<ProduitDto?> GetByNameAsync(string name);
    }
}
