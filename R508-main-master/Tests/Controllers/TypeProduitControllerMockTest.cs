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
    public class TypeProduitControllerMockTests
    {
        private Mock<IDataRepository<TypeProduit>> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private TypeProduitController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IDataRepository<TypeProduit>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new TypeProduitController(_mockRepo.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task GetAll_ReturnsNoContent_WhenEmpty()
        {
            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<TypeProduit>>(new List<TypeProduit>()));

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task GetAll_ReturnsOk_WhenDataExists()
        {
            var produits = new List<TypeProduit> { new TypeProduit { IdTypeProduit = 1, NomTypeProduit = "Chaussures" } };
            var dtos = new List<TypeProduitDto> { new TypeProduitDto { IdTypeProduit = 1, NomTypeProduit = "Chaussures" } };

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new ActionResult<IEnumerable<TypeProduit>>(produits));
            _mockMapper.Setup(m => m.Map<IEnumerable<TypeProduitDto>>(produits)).Returns(dtos);

            var result = await _controller.GetAll();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as IEnumerable<TypeProduitDto>;
            Assert.AreEqual(1, returnValue.Count());
            Assert.AreEqual("Chaussures", returnValue.First().NomTypeProduit);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new ActionResult<TypeProduit>((TypeProduit)null));

            var result = await _controller.Get(10);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOk_WhenExists()
        {
            var entity = new TypeProduit { IdTypeProduit = 2, NomTypeProduit = "Vêtements" };
            var dto = new TypeProduitDto { IdTypeProduit = 2, NomTypeProduit = "Vêtements" };

            _mockRepo.Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(new ActionResult<TypeProduit>(entity));
            _mockMapper.Setup(m => m.Map<TypeProduitDto>(entity)).Returns(dto);

            var result = await _controller.Get(2);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnValue = okResult.Value as TypeProduitDto;
            Assert.AreEqual("Vêtements", returnValue.NomTypeProduit);
        }

        [TestMethod]
        public async Task Post_ReturnsCreatedAtAction()
        {
            var dto = new TypeProduitDto { IdTypeProduit = 3, NomTypeProduit = "Accessoires" };
            var entity = new TypeProduit { IdTypeProduit = 3, NomTypeProduit = "Accessoires" };

            _mockMapper.Setup(m => m.Map<TypeProduit>(dto)).Returns(entity);
            _mockMapper.Setup(m => m.Map<TypeProduitDto>(entity)).Returns(dto);

            var result = await _controller.Post(dto);

            var created = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(created);
            var returnValue = created.Value as TypeProduitDto;
            Assert.AreEqual("Accessoires", returnValue.NomTypeProduit);
        }

        [TestMethod]
        public async Task Put_ReturnsNotFound_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(4))
                .ReturnsAsync(new ActionResult<TypeProduit>((TypeProduit)null));

            var result = await _controller.Put(4, new TypeProduitDto());

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        public async Task Put_ReturnsNoContent_WhenUpdated()
        {
            var existing = new TypeProduit { IdTypeProduit = 5, NomTypeProduit = "Ancien" };
            var dto = new TypeProduitDto { IdTypeProduit = 5, NomTypeProduit = "Nouveau" };
            var updated = new TypeProduit { IdTypeProduit = 5, NomTypeProduit = "Nouveau" };

            _mockRepo.Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(new ActionResult<TypeProduit>(existing));

            _mockMapper.Setup(m => m.Map(It.IsAny<TypeProduitDto>(), It.IsAny<TypeProduit>()))
                .Returns(updated);

            var result = await _controller.Put(5, dto);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            // Vérifie que UpdateAsync a été appelé avec existing et un objet mis à jour
            _mockRepo.Verify(r => r.UpdateAsync(existing, It.IsAny<TypeProduit>()), Times.Once);
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFound_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(6))
                .ReturnsAsync(new ActionResult<TypeProduit>((TypeProduit)null));

            var result = await _controller.Delete(6);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            var entity = new TypeProduit { IdTypeProduit = 7, NomTypeProduit = "ASupprimer" };

            _mockRepo.Setup(r => r.GetByIdAsync(7))
                .ReturnsAsync(new ActionResult<TypeProduit>(entity));

            var result = await _controller.Delete(7);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _mockRepo.Verify(r => r.DeleteAsync(entity), Times.Once);
        }
    }
}
