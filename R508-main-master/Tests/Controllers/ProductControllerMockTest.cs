using App.Controllers;
using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;

namespace Tests.Controllers;

[TestClass]
[TestSubject(typeof(ProductController))]
[TestCategory("mock")]
public class ProductControllerMockTest
{
    private readonly ProductController _productController;
    private readonly Mock<IDataRepository<Produit>> _produitManager;
    private readonly Mock<IMapper> _mapper;
    private readonly AppDbContext _context;

    public ProductControllerMockTest()
    {
        _produitManager = new Mock<IDataRepository<Produit>>();
        _mapper = new Mock<IMapper>();
        _productController = new ProductController(_produitManager.Object, _mapper.Object, null);
    }

    [TestMethod]
    public void ShouldGetProduct()
    {
        // Given : un produit existant
        var produitInDb = new Produit { IdProduit = 1, NomProduit = "Chaise", Description = "Superbe chaise" };
        var produitDto = new ProduitDetailDto { IdProduit = 1, NomProduit = "Chaise", Description = "Superbe chaise" };

        _produitManager.Setup(m => m.GetByIdAsync(1)).ReturnsAsync(new ActionResult<Produit>(produitInDb));
        _mapper.Setup(m => m.Map<ProduitDetailDto>(produitInDb)).Returns(produitDto);

        // When : appel GET
        var action = _productController.Get(1).GetAwaiter().GetResult();

        // Then : on obtient OK + DTO
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var ok = action.Result as OkObjectResult;
        var dto = ok?.Value as ProduitDetailDto;
        Assert.AreEqual(produitDto.NomProduit, dto.NomProduit);
    }

    [TestMethod]
    public void GetProductShouldReturnNotFound()
    {
        // Given : produit inexistant
        _produitManager.Setup(m => m.GetByIdAsync(30)).ReturnsAsync(new ActionResult<Produit>((Produit)null));

        // When
        var action = _productController.Get(30).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        Assert.IsNull(action.Value);
    }

    [TestMethod]
    public void ShouldGetAllProducts()
    {
        // Given : plusieurs produits
        var produits = new List<Produit>
        {
            new Produit { IdProduit = 1, NomProduit = "Chaise" },
            new Produit { IdProduit = 2, NomProduit = "Armoire" }
        };
        var produitsDto = produits.Select(p => new ProduitDto { IdProduit = p.IdProduit, NomProduit = p.NomProduit }).ToList();

        _produitManager.Setup(m => m.GetAllAsync()).ReturnsAsync(new ActionResult<IEnumerable<Produit>>(produits));
        _mapper.Setup(m => m.Map<IEnumerable<ProduitDto>>(produits)).Returns(produitsDto);

        // When
        var action = _productController.GetAll().GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var ok = action.Result as OkObjectResult;
        var returnList = ok?.Value as IEnumerable<ProduitDto>;
        Assert.AreEqual(2, returnList.Count());
    }

    [TestMethod]
    public void ShouldDeleteProduct()
    {
        // Given : produit existant
        var produitInDb = new Produit { IdProduit = 2, NomProduit = "Chaise" };
        _produitManager.Setup(m => m.GetByIdAsync(2)).ReturnsAsync(new ActionResult<Produit>(produitInDb));

        // When
        var action = _productController.Delete(2).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        _produitManager.Verify(m => m.DeleteAsync(produitInDb), Times.Once);
    }

    [TestMethod]
    public void ShouldNotDeleteProductBecauseNotFound()
    {
        // Given : produit inexistant
        _produitManager.Setup(m => m.GetByIdAsync(999)).ReturnsAsync(new ActionResult<Produit>((Produit)null));

        // When
        var action = _productController.Delete(999).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldCreateProduct()
    {
        // Given : DTO à insérer
        var dto = new ProduitDetailDto { IdProduit = 10, NomProduit = "Chaise", Marque = "IKEA", Type = "Meuble" };
        var produit = new Produit { IdProduit = 10, NomProduit = "Chaise", IdMarque = 1, IdTypeProduit = 1 };

        _mapper.Setup(m => m.Map<Produit>(dto)).Returns(produit);
        _mapper.Setup(m => m.Map<ProduitDetailDto>(produit)).Returns(dto);
        _produitManager.Setup(m => m.AddAsync(produit)).Returns(Task.CompletedTask);

        // When
        var action = _productController.Create(dto).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        _produitManager.Verify(m => m.AddAsync(produit), Times.Once);
    }

    [TestMethod]
    public void ShouldUpdateProduct()
    {
        // Given : produit existant + DTO maj
        var produit = new Produit { IdProduit = 20, NomProduit = "Bureau" };
        var dto = new ProduitDetailDto { IdProduit = 20, NomProduit = "Lit", Marque = "IKEA", Type = "Meuble" };

        _produitManager.Setup(m => m.GetByIdAsync(20))
            .ReturnsAsync(new ActionResult<Produit>(produit));

        var updatedProduit = new Produit { IdProduit = 20, NomProduit = "Lit" };
        _mapper.Setup(m => m.Map(dto, produit)).Returns(updatedProduit);

        _produitManager.Setup(m => m.UpdateAsync(produit, updatedProduit))
            .Returns(Task.CompletedTask);

        // When
        var action = _productController.Put(20, dto).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        _produitManager.Verify(m => m.UpdateAsync(produit, updatedProduit), Times.Once);
    }


    [TestMethod]
    public void ShouldNotUpdateProductBecauseNotFound()
    {
        // Given : produit inexistant
        var dto = new ProduitDetailDto { IdProduit = 99, NomProduit = "Lit" };
        _produitManager.Setup(m => m.GetByIdAsync(99)).ReturnsAsync(new ActionResult<Produit>((Produit)null));

        // When
        var action = _productController.Put(99, dto).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        _produitManager.Verify(m => m.UpdateAsync(It.IsAny<Produit>(), It.IsAny<Produit>()), Times.Never);
    }

    [TestMethod]
    public void ShouldGetProductsByType()
    {
        // Given : produits avec types
        var produits = new List<Produit>
        {
            new Produit { IdProduit = 1, NomProduit = "Chaise", TypeProduitNavigation = new TypeProduit { IdTypeProduit = 5 } },
            new Produit { IdProduit = 2, NomProduit = "Armoire", TypeProduitNavigation = new TypeProduit { IdTypeProduit = 10 } }
        };
        var dtoList = new List<ProduitDto> { new ProduitDto { IdProduit = 1, NomProduit = "Chaise" } };

        _produitManager.Setup(m => m.GetAllAsync()).ReturnsAsync(new ActionResult<IEnumerable<Produit>>(produits));
        _mapper.Setup(m => m.Map<IEnumerable<ProduitDto>>(It.IsAny<IEnumerable<Produit>>())).Returns(dtoList);

        // When
        var action = _productController.GetByType(5).GetAwaiter().GetResult();

        // Then
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var ok = action.Result as OkObjectResult;
        var resultList = ok?.Value as IEnumerable<ProduitDto>;
        Assert.AreEqual(1, resultList.Count());
    }
}
