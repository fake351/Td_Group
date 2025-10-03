using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Controllers;
using App.DTO;
using App.Models;
using App.Models.Repository;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Tests.Utils;
using App.Models.EntityFramework; // pour InMemoryRepository

namespace App.Tests.Controllers
{
    [TestClass]
    [UsedImplicitly]
    public class ProductControllerTests
    {
        private InMemoryRepository<Produit> _repo;
        private IMapper _mapper;
        private AppDbContext _context;
        private ProductController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Repo mémoire
            _repo = new InMemoryRepository<Produit>();

            // AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Produit, ProduitDto>().ReverseMap();
                cfg.CreateMap<Marque, MarqueDto>().ReverseMap();
                cfg.CreateMap<Produit, ProduitDetailDto>().ReverseMap();
                cfg.CreateMap<TypeProduit, TypeProduitDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            // DbContext EF Core InMemory
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql("Host=localhost;Database=ma_base;Username=mon_user;Password=mon_mdp;SearchPath=test")
                .Options;

            _context = new AppDbContext(options);

            // Controller à tester
            _controller = new ProductController(_repo, _mapper, _context);
        }


        [TestMethod]
        public async Task GetAll_ReturnsNoContent_WhenEmpty()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAll_ReturnsOk_WhenDataExists()
        {
            _repo.Add(new Produit { IdProduit = 1, NomProduit = "Chaussures" });

            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var produits = okResult.Value as IEnumerable<ProduitDto>;
            Assert.AreEqual(1, produits.Count());
            Assert.AreEqual("Chaussures", produits.First().NomProduit);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.Get(42);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenExists()
        {
            _repo.Add(new Produit { IdProduit = 2, NomProduit = "T-shirt" });

            var result = await _controller.Get(2);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var produit = okResult.Value as ProduitDetailDto;
            Assert.AreEqual("T-shirt", produit.NomProduit);
        }
        [TestMethod]
        public async Task Post_CreatesNewProduct()
        {
            // Arrange: créer une Marque obligatoire
            var marque = new Marque { NomMarque = "Nike" };
            _context.Marques.Add(marque);

            // Créer un TypeProduit obligatoire
            var type = new TypeProduit { NomTypeProduit = "Vêtements" };
            _context.TypeProduits.Add(type);

            await _context.SaveChangesAsync();

            var dto = new ProduitDetailDto
            {
                NomProduit = "Pantalon",
                Marque = marque.NomMarque,       // référence la marque existante
                Type = type.NomTypeProduit // référence le type existant
            };

            // Act
            var result = await _controller.Create(dto);
            var created = result.Result as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(created);
            var produit = created.Value as ProduitDetailDto;
            Assert.AreEqual("Pantalon", produit.NomProduit);
        }

        [TestMethod]
        public async Task Put_ReturnsNotFound_WhenNotExists()
        {
            var dto = new ProduitDetailDto { IdProduit = 4, NomProduit = "Casquette" };

            var result = await _controller.Put(4, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Put_UpdatesExistingProduct()
        {
            // Arrange: créer une Marque obligatoire
            var marque = new Marque { NomMarque = "Nike" };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();

            var produit = new Produit {  NomProduit = "Old", IdMarque = marque.IdMarque, Description = "Produit test",NomPhoto="Phototest" ,UriPhoto="aaa"};
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            var dto = new ProduitDetailDto {  NomProduit = "New", Marque = marque.NomMarque, Description = "Produit test", NomPhoto = "Phototest", UriPhoto = "aaa" };

            // Act
            var result = await _controller.Put(5, dto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            var updated = _context.Produits.First(p => p.IdProduit == 5);
            Assert.AreEqual("New", updated.NomProduit);
        }


        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.Delete(6);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_RemovesExistingProduct()
        {
            _repo.Add(new Produit { IdProduit = 7, NomProduit = "ToDelete" });

            var result = await _controller.Delete(7);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual(0, _repo.GetAll().Count());
        }
    }
}
