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
    public class MarqueControllerTests
    {
        private InMemoryRepository<Marque> _repo;
        private IMapper _mapper;
        private MarqueController _controller;

        [TestInitialize]
        public void Setup()
        {
            _repo = new InMemoryRepository<Marque>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Marque, MarqueDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _controller = new MarqueController(_repo, _mapper);
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
            _repo.Add(new Marque { IdMarque = 1, NomMarque = "Nike" });

            var result = await _controller.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var marques = okResult.Value as IEnumerable<MarqueDto>;
            Assert.AreEqual(1, marques.Count());
            Assert.AreEqual("Nike", marques.First().NomMarque);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.Get(99);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenExists()
        {
            _repo.Add(new Marque { IdMarque = 2, NomMarque = "Adidas" });

            var result = await _controller.Get(2);
            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            var marque = okResult.Value as MarqueDto;
            Assert.AreEqual("Adidas", marque.NomMarque);
        }

        [TestMethod]
        public async Task Post_CreatesNewMarque()
        {
            var dto = new MarqueDto { IdMarque = 3, NomMarque = "Puma" };

            var result = await _controller.Post(dto);
            var created = result.Result as CreatedAtActionResult;

            Assert.IsNotNull(created);
            var marque = created.Value as MarqueDto;
            Assert.AreEqual("Puma", marque.NomMarque);
        }

        [TestMethod]
        public async Task Put_ReturnsNotFound_WhenNotExists()
        {
            var dto = new MarqueDto { IdMarque = 4, NomMarque = "Reebok" };

            var result = await _controller.Put(4, dto);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Put_UpdatesExistingMarque()
        {
            _repo.Add(new Marque { IdMarque = 5, NomMarque = "Old" });
            var dto = new MarqueDto { IdMarque = 5, NomMarque = "New" };

            var result = await _controller.Put(5, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual("New", _repo.GetAll().First().NomMarque);
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.Delete(6);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_RemovesExistingMarque()
        {
            _repo.Add(new Marque { IdMarque = 7, NomMarque = "ToDelete" });

            var result = await _controller.Delete(7);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.AreEqual(0, _repo.GetAll().Count());
        }
    }

}
