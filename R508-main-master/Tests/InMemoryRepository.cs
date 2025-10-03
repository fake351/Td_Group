using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using App.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace App.Tests.Utils
{
    /// <summary>
    /// Implémentation générique en mémoire de IDataRepository<T>.
    /// Utilisable pour les tests unitaires à la place d'une vraie base de données.
    /// </summary>
    public class InMemoryRepository<T> : IDataRepository<T> where T : class
    {
        private readonly List<T> _storage = new();

        public Task<ActionResult<IEnumerable<T>>> GetAllAsync()
            => Task.FromResult<ActionResult<IEnumerable<T>>>(_storage.ToList());

        public Task<ActionResult<T?>> GetByIdAsync(int id)
        {
            var entity = _storage.FirstOrDefault(e => GetIdValue(e) == id);
            return Task.FromResult<ActionResult<T?>>(entity);
        }

        /// <summary>
        /// Recherche une entité par une valeur string (ex : NomMarque, NomTypeProduit).
        /// On prend la première propriété string trouvée.
        /// </summary>
        public Task<ActionResult<T?>> GetByStringAsync(string str)
        {
            var prop = typeof(T).GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(string));

            if (prop == null)
                throw new InvalidOperationException($"L'entité {typeof(T).Name} n'a pas de propriété string.");

            var entity = _storage.FirstOrDefault(e =>
                string.Equals((string?)prop.GetValue(e), str, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult<ActionResult<T?>>(entity);
        }

        public Task AddAsync(T entity)
        {
            _storage.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entityToUpdate, T entity)
        {
            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    var value = prop.GetValue(entity);
                    prop.SetValue(entityToUpdate, value);
                }
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _storage.Remove(entity);
            return Task.CompletedTask;
        }

        // Méthodes utilitaires pour initialiser/inspecter le contenu
        public void Add(T entity) => _storage.Add(entity);
        public IEnumerable<T> GetAll() => _storage;

        private static int GetIdValue(T entity)
        {
            var prop = typeof(T).GetProperties()
                .FirstOrDefault(p => p.Name.StartsWith("Id", StringComparison.OrdinalIgnoreCase) && p.PropertyType == typeof(int));

            if (prop == null)
                throw new InvalidOperationException($"L'entité {typeof(T).Name} n'a pas de propriété 'Id...' de type int.");

            return (int)prop.GetValue(entity)!;
        }
    }
}
