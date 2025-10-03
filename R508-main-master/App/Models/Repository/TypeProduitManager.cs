using App.Models.Repository;
using App.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;

namespace App.Models.DataManager
{
    public class TypeProduitManager : IDataRepository<TypeProduit>
    {
        private readonly AppDbContext _context;

        public TypeProduitManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<TypeProduit>> GetByIdAsync(int id)
        {
            return await _context.TypeProduits.FirstOrDefaultAsync(t => t.IdTypeProduit == id);
        }

        public async Task<ActionResult<IEnumerable<TypeProduit>>> GetAllAsync()
        {
            return await _context.TypeProduits.ToListAsync();
        }

        public async Task AddAsync(TypeProduit entity)
        {
            _context.TypeProduits.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TypeProduit oldEntity, TypeProduit newEntity)
        {
            _context.Entry(oldEntity).CurrentValues.SetValues(newEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TypeProduit entity)
        {
            _context.TypeProduits.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<ActionResult<TypeProduit?>> GetByStringAsync(string str)
        {
            // Exemple : recherche par le nom du type de produit
            var entity = await _context.TypeProduits
                .FirstOrDefaultAsync(tp => tp.NomTypeProduit == str);

            if (entity == null)
                return new NotFoundResult();

            return entity;
        }
    }
}
