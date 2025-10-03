using BlazorApp.Models;
using System.Net.Http.Json;

namespace BlazorApp.ViewModels
{
    public class MarqueViewModel
    {
        private readonly HttpClient _http;

        public MarqueViewModel(HttpClient http)
        {
            _http = http;
        }

        public List<MarqueDto>? Marques { get; private set; }

        // --- Propriété pour l'édition / ajout ---
        public MarqueDto? EditingMarque { get; set; }

        // --- Popup Détails ---
        public bool IsDetailsPopupVisible { get; private set; } = false;
        public List<ProduitDto>? SelectedMarqueProduits { get; private set; }
        public string? SelectedMarqueNom { get; private set; }

        // --- Chargement des marques ---
        public async Task LoadMarques()
        {
            Marques = await _http.GetFromJsonAsync<List<MarqueDto>>("http://localhost:5128/api/marques");
        }

        public void NewMarque()
        {
            EditingMarque = new MarqueDto();
        }

        public void CancelEdit()
        {
            EditingMarque = null;
        }

        public async Task SaveMarque()
        {
            if (EditingMarque == null) return;

            if (EditingMarque.IdMarque == 0)
            {
                await _http.PostAsJsonAsync("http://localhost:5128/api/marques", EditingMarque);
            }
            else
            {
                await _http.PutAsJsonAsync($"http://localhost:5128/api/marques/{EditingMarque.IdMarque}", EditingMarque);
            }

            await LoadMarques();
            CancelEdit();
        }

        public async Task DeleteMarque(int id)
        {
            await _http.DeleteAsync($"http://localhost:5128/api/marques/{id}");
            await LoadMarques();
        }

        // --- Popup détails ---
        public async Task ShowDetails(int marqueId, string marqueNom)
        {
            SelectedMarqueNom = marqueNom;
            SelectedMarqueProduits = await _http.GetFromJsonAsync<List<ProduitDto>>(
                $"http://localhost:5128/api/produits/{marqueId}/produits"
            );
            IsDetailsPopupVisible = true;
        }

        public void CloseDetails()
        {
            IsDetailsPopupVisible = false;
            SelectedMarqueProduits = null;
            SelectedMarqueNom = null;
        }

    }
}
