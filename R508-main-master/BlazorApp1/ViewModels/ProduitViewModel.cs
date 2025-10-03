using System.Net.Http.Json;
using BlazorApp.Models;

namespace BlazorApp.ViewModels
{
    public class ProduitViewModel
    {
        private readonly HttpClient _http;

        public List<ProduitDto>? Produits { get; set; }
        public ProduitDetailDto? SelectedProduct { get; set; }
        public ProduitDetailDto? EditingProduct { get; set; }
        public bool IsDetailsPopupVisible { get; set; }

        public ProduitViewModel(HttpClient http)
        {
            _http = http;
        }

        public async Task LoadProduits()
        {
            Produits = await _http.GetFromJsonAsync<List<ProduitDto>>("http://localhost:5128/api/Produits");
        }

        public async Task LoadProduitDetail(int id)
        {
            SelectedProduct = await _http.GetFromJsonAsync<ProduitDetailDto>($"http://localhost:5128/api/Produits/{id}");
            IsDetailsPopupVisible = true;
        }

        public void ShowDetails(int id)
        {
            _ = LoadProduitDetail(id);
        }

        public void CloseDetails()
        {
            SelectedProduct = null;
            IsDetailsPopupVisible = false;
        }

        public async Task EditProduct(int id)
        {
            var produitDetail = await _http.GetFromJsonAsync<ProduitDetailDto>($"http://localhost:5128/api/Produits/{id}");
            if (produitDetail != null)
            {
                EditingProduct = produitDetail;
            }
        }

        public void NewProduit()
        {
            EditingProduct = new ProduitDetailDto();
        }

        public void CancelEdit()
        {
            EditingProduct = null;
        }

        public async Task DeleteProduit(int id)
        {
            await _http.DeleteAsync($"http://localhost:5128/api/Produits/{id}");
            await LoadProduits();
        }

        public async Task SaveProduit()
        {
            if (EditingProduct == null) return;

            // --- Préparer le DTO à envoyer côté API ---
            var dtoToSend = new ProduitDetailDto
            {
                IdProduit = EditingProduct.IdProduit,
                NomProduit = EditingProduct.NomProduit,
                Description = EditingProduct.Description,
                Type = EditingProduct.Type,
                Marque = EditingProduct.Marque,
                Nomphoto = EditingProduct.Nomphoto,
                Uriphoto = EditingProduct.Nomphoto +".jpg",
                StockReel = EditingProduct.StockReel,
                StockMin = EditingProduct.StockMin,
                StockMax = EditingProduct.StockMax
            };

            HttpResponseMessage response;

            if (EditingProduct.IdProduit == 0)
            {
                // Nouveau produit : le serveur créera la marque/type si elles n'existent pas
                response = await _http.PostAsJsonAsync("http://localhost:5128/api/Produits", dtoToSend);
            }
            else
            {
                // Modification produit
                response = await _http.PutAsJsonAsync($"http://localhost:5128/api/Produits/{EditingProduct.IdProduit}", dtoToSend);
            }

            if (response.IsSuccessStatusCode)
            {
                EditingProduct = null;
                await LoadProduits();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                // Afficher l'erreur ou logger
                Console.WriteLine($"Erreur API : {error}");
            }
        }

    }
}
