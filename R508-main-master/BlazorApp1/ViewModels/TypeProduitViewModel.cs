using BlazorApp.Models;
using System.Net.Http.Json;

namespace BlazorApp.ViewModels
{
    public class TypeProduitViewModel
    {
        private readonly HttpClient _http;

        public TypeProduitViewModel(HttpClient http)
        {
            _http = http;
        }

        // Liste des types
        public List<TypeProduitDto>? Types { get; private set; }

        // Type en cours de modification/ajout
        public TypeProduitDto? EditingType { get; private set; }

        // Pour la popup Détails
        public bool IsDetailsPopupVisible { get; private set; } = false;
        public List<ProduitDto>? SelectedTypeProduits { get; private set; }
        public string? SelectedTypeNom { get; private set; }

        // --- Chargement des types ---
        public async Task LoadTypes()
        {
            Types = await _http.GetFromJsonAsync<List<TypeProduitDto>>("http://localhost:5128/api/types-produits");
        }

        // --- Gestion ajout / édition ---
        public void NewType()
        {
            EditingType = new TypeProduitDto();
        }

        public async Task EditType(int id)
        {
            var type = await _http.GetFromJsonAsync<TypeProduitDto>($"http://localhost:5128/api/types-produits/{id}");
            if (type != null)
            {
                EditingType = type;
            }
        }

        public void CancelEdit()
        {
            EditingType = null;
        }

        public async Task SaveType()
        {
            if (EditingType == null) return;

            if (EditingType.IdTypeProduit == 0)
            {
                await _http.PostAsJsonAsync("http://localhost:5128/api/types-produits", EditingType);
            }
            else
            {
                await _http.PutAsJsonAsync($"http://localhost:5128/api/types-produits/{EditingType.IdTypeProduit}", EditingType);
            }

            await LoadTypes();
            CancelEdit();
        }

        // --- Suppression ---
        public async Task DeleteType(int id)
        {
            await _http.DeleteAsync($"http://localhost:5128/api/types-produits/{id}");
            await LoadTypes();
        }

        // --- Popup détails ---
        public async Task ShowDetails(int typeId, string typeNom)
        {
            SelectedTypeNom = typeNom;
            SelectedTypeProduits = await _http.GetFromJsonAsync<List<ProduitDto>>(
                $"http://localhost:5128/api/produits/bytype/{typeId}"
            );
            IsDetailsPopupVisible = true;
        }

        public void CloseDetails()
        {
            IsDetailsPopupVisible = false;
            SelectedTypeProduits = null;
            SelectedTypeNom = null;
        }
    }
}
