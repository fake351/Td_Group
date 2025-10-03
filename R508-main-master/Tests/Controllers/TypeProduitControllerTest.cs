using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Controllers;
using App.DTO;
using App.Models;
using App.Models.Repository;
using App.Tests.Utils;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace App.Tests.Controllers
{
    [TestClass]
    [UsedImplicitly]
    public class TypeProduitControllerTests
    {
        private InMemoryRepository<TypeProduit> _repo;
        private IMapper _mapper;
        private TypeProduitController _controller;

        [TestInitialize]
        public void Setup()
        {
            _repo = new InMemoryRepository<TypeProduit>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TypeProduit, TypeProduitDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _controller = new TypeProduitController(_repo, _mapper);
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
            _repo.Add(new TypeProduit { IdTypeProduit = 1, NomTypeProduit = "Chaussures" });

            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var produits = okResult.Value as IEnumerable<TypeProduitDto>;
            Assert.AreEqual(1, produits.Count());
            Assert.AreEqual("Chaussures", produits.First().NomTypeProduit);
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
            _repo.Add(new TypeProduit { IdTypeProduit = 2, NomTypeProduit = "Vêtements" });

            var result = await _controller.Get(2);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var produit = okResult.Value as TypeProduitDto;
            Assert.AreEqual("Vêtements", produit.NomTypeProduit);
        }

        [TestMethod]
        public async Task Post_CreatesNewTypeProduit()
        {
            var dto = new TypeProduitDto { IdTypeProduit = 3, NomTypeProduit = "Accessoires" };

            var result = await _controller.Post(dto);
            var created = result.Result as CreatedAtActionResult;

            Assert.IsNotNull(created);
            var produit = created.Value as TypeProduitDto;
            Assert.AreEqual("Accessoires", produit.NomTypeProduit);
        }

        [TestMethod]
        public async Task Put_ReturnsNotFound_WhenNotExists()
        {
            var dto = new TypeProduitDto { IdTypeProduit = 4, NomTypeProduit = "Montres" };

            var result = await _controller.Put(4, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Put_UpdatesExistingTypeProduit()
        {
            _repo.Add(new TypeProduit { IdTypeProduit = 5, NomTypeProduit = "Ancien" });
            var dto = new TypeProduitDto { IdTypeProduit = 5, NomTypeProduit = "Nouveau" };

            var result = await _controller.Put(5, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual("Nouveau", _repo.GetAll().First().NomTypeProduit);
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.Delete(6);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_RemovesExistingTypeProduit()
        {
            _repo.Add(new TypeProduit { IdTypeProduit = 7, NomTypeProduit = "ASupprimer" });

            var result = await _controller.Delete(7);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual(0, _repo.GetAll().Count());
        }
    }
}
