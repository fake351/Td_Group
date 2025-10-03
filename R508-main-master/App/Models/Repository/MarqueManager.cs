using App.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Models.Repository
{
    public class MarqueManager(AppDbContext context) : IDataRepository<Marque>
    {
        public async Task<ActionResult<IEnumerable<Marque>>> GetAllAsync()
        {
            var marques = await context.Marques.ToListAsync();
            return marques;
        }

        public async Task<ActionResult<Marque?>> GetByIdAsync(int id)
        {
            return await context.Marques.FindAsync(id);
        }

        public async Task<ActionResult<Marque?>> GetByStringAsync(string str)
        {
            return await context.Marques.FirstOrDefaultAsync(m => m.NomMarque == str);
        }

        public async Task AddAsync(Marque entity)
        {
            await context.Marques.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Marque entityToUpdate, Marque entity)
        {
            context.Marques.Attach(entityToUpdate);
            context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Marque entity)
        {
            context.Marques.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
