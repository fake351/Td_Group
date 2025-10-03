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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace App.Tests.Controllers
{
    [TestClass]
    [UsedImplicitly]
    public class MarqueControllerMockTests
    {
        private Mock<IDataRepository<Marque>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private MarqueController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IDataRepository<Marque>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new MarqueController(_mockRepo.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task GetAll_ReturnsNoContent_WhenEmpty()
        {
            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Marque>>(new List<Marque>()));

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAll_ReturnsOk_WhenDataExists()
        {
            var marques = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "Nike" } };
            var dtos = new List<MarqueDto> { new MarqueDto { IdMarque = 1, NomMarque = "Nike" } };

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<Marque>>(marques));
            _mockMapper.Setup(m => m.Map<IEnumerable<MarqueDto>>(marques)).Returns(dtos);

            var result = await _controller.GetAll();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as IEnumerable<MarqueDto>;
            Assert.AreEqual(1, returnValue.Count());
            Assert.AreEqual("Nike", returnValue.First().NomMarque);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new ActionResult<Marque>((Marque)null));

            var result = await _controller.Get(1);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenExists()
        {
            var marque = new Marque { IdMarque = 1, NomMarque = "Adidas" };
            var dto = new MarqueDto { IdMarque = 1, NomMarque = "Adidas" };

            _mockRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new ActionResult<Marque>(marque));
            _mockMapper.Setup(m => m.Map<MarqueDto>(marque)).Returns(dto);

            var result = await _controller.Get(1);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as MarqueDto;
            Assert.AreEqual("Adidas", returnValue.NomMarque);
        }

        [TestMethod]
        public async Task Post_ReturnsCreatedAtAction()
        {
            var dto = new MarqueDto { IdMarque = 2, NomMarque = "Puma" };
            var entity = new Marque { IdMarque = 2, NomMarque = "Puma" };

            _mockMapper.Setup(m => m.Map<Marque>(dto)).Returns(entity);
            _mockMapper.Setup(m => m.Map<MarqueDto>(entity)).Returns(dto);

            var result = await _controller.Post(dto);

            var createdAt = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAt);
            var returnValue = createdAt.Value as MarqueDto;
            Assert.AreEqual("Puma", returnValue.NomMarque);
        }

        [TestMethod]
        public async Task Put_ReturnsNotFound_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(3))
                .ReturnsAsync(new ActionResult<Marque>((Marque)null));

            var result = await _controller.Put(3, new MarqueDto());

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Put_ReturnsNoContent_WhenUpdated()
        {
            var existing = new Marque { IdMarque = 4, NomMarque = "Old" };
            var dto = new MarqueDto { IdMarque = 4, NomMarque = "New" };
            var updated = new Marque { IdMarque = 4, NomMarque = "New" };

            _mockRepo.Setup(r => r.GetByIdAsync(4))
                .ReturnsAsync(new ActionResult<Marque>(existing));
            _mockMapper.Setup(m => m.Map(dto, existing)).Returns(updated);

            var result = await _controller.Put(4, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockRepo.Verify(r => r.UpdateAsync(existing, updated), Times.Once);
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(new ActionResult<Marque>((Marque)null));

            var result = await _controller.Delete(5);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            var entity = new Marque { IdMarque = 6, NomMarque = "DeleteMe" };

            _mockRepo.Setup(r => r.GetByIdAsync(6))
                .ReturnsAsync(new ActionResult<Marque>(entity));

            var result = await _controller.Delete(6);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockRepo.Verify(r => r.DeleteAsync(entity), Times.Once);
        }
    }
}
