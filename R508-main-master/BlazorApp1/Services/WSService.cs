using BlazorApp.Models;
using System.Net.Http.Json;
using BlazorApp.Services;

namespace BlazorApp.Service;

public class WSService : IService<ProduitDto>
{
    private readonly HttpClient httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5011/api/")
    };

    public async Task AddAsync(ProduitDto produit)
    {
        await httpClient.PostAsJsonAsync<ProduitDto>("produits", produit);
    }

    public async Task DeleteAsync(int id)
    {
        await httpClient.DeleteAsync($"produits/{id}");
    }

    public async Task<List<ProduitDto>?> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<ProduitDto>?>("produits");
    }

    public async Task<ProduitDto?> GetByIdAsync(int id)
    {
        return await httpClient.GetFromJsonAsync<ProduitDto?>($"produits/{id}");
    }

    public async Task<ProduitDto?> GetByNameAsync(string name)
    {
        var response = await httpClient.PostAsJsonAsync("produits/search", name);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProduitDto>();
    }

    public async Task UpdateAsync(ProduitDto updatedEntity)
    {
        await httpClient.PutAsJsonAsync<ProduitDto>($"produits/{updatedEntity.IdProduit}", updatedEntity);
    }
}
