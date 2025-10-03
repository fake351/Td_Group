namespace BlazorApp.Models
{
    public class ProduitDetailDto
    {
        public int IdProduit { get; set; }
        public string? NomProduit { get; set; }
        public string? Type { get; set; }
        public string? Marque { get; set; }
        public string? Description { get; set; }
        public string? Nomphoto { get; set; }
        public string? Uriphoto { get; set; }
        public int StockReel { get; set; }
        public int StockMin { get; set; }
        public int StockMax { get; set; }
        public bool EnReappro => StockReel <= StockMin;
    }
}
