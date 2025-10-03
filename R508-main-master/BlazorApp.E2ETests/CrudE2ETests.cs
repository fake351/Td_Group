using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace BlazorApp.E2ETests
{
    [TestClass]
    public class CrudE2ETests
    {
        private static IPlaywright _playwright;
        private static IBrowser _browser;

        [ClassInitialize]
        public static async Task Setup(TestContext context)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, // mets true pour exécuter sans fenêtre
                SlowMo = 200      // ralentit un peu pour voir les actions
            });
        }

        [ClassCleanup]
        public static async Task Teardown()
        {
            await _browser.DisposeAsync();
            _playwright.Dispose();
        }

        private async Task RunCrudTest(string url, string entityName, string newName, string updatedName, bool isProduit = false)
        {
            var context = await _browser.NewContextAsync();
            var page = await context.NewPageAsync();

            // Aller sur la page Blazor
            await page.GotoAsync(url);

            // 1. Création
            await page.ClickAsync("button:has-text('Nouveau')");

            if (isProduit)
            {
                // Produits : remplir tous les champs obligatoires
                await page.GetByPlaceholder("Nom Produit").FillAsync("TestProduit");
                await page.GetByPlaceholder("Saisir un type").FillAsync("TestType");
                await page.GetByPlaceholder("Saisir une marque").FillAsync("TestMarque");
                await page.GetByPlaceholder("Description").FillAsync("Chaise test");
                await page.GetByPlaceholder("Nom Photo").FillAsync("phototest");
            }
            else
            {
                // Marques & Types
                await page.FillAsync("input", newName);
            }

            await page.ClickAsync("button:has-text('Enregistrer')");

            // Vérifier création
            await page.WaitForSelectorAsync($"text={newName}");

            // 2. Modification
            await page.ClickAsync("button:has-text('Modifier')");
            await page.FillAsync("input", updatedName);
            await page.ClickAsync("button:has-text('Enregistrer')");

            await page.WaitForSelectorAsync($"text={updatedName}");

            // 3. Suppression
            await page.ClickAsync("button:has-text('Supprimer')");

            // ⚠️ reload car la page ne supprime pas du DOM
            await page.ReloadAsync();

            await page.WaitForSelectorAsync($"text={updatedName}", new PageWaitForSelectorOptions
            {
                State = WaitForSelectorState.Detached
            });

            await context.CloseAsync();
        }

        [TestMethod]
        public async Task Crud_Marques()
        {
            await RunCrudTest("http://localhost:5063/marques", "Marque", "TestMarque", "TestMarqueModifiee");
        }

        [TestMethod]
        public async Task Crud_Produits()
        {
            await RunCrudTest("http://localhost:5063/produits", "Produit", "TestProduit", "TestProduitModifie", isProduit: true);
        }

        [TestMethod]
        public async Task Crud_TypesProduits()
        {
            await RunCrudTest("http://localhost:5063/typesproduits", "Type", "TestType", "TestTypeModifie");
        }
    }
}
