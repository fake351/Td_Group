using App.DTO;
using App.Models;
using App.Models.EntityFramework;
using App.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;


namespace App.Controllers;

[Route("api/produits")]
[ApiController]
public class ProductController : ControllerBase
{ 
    private readonly IDataRepository<Produit> _manager;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public ProductController(IDataRepository<Produit> manager, IMapper mapper,AppDbContext context)
    {
        _manager = manager;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProduitDetailDto?>> Get(int id)
    {
        var result = await _manager.GetByIdAsync(id);
        if(result.Value == null)
            return NotFound();
        var dto = _mapper.Map<ProduitDetailDto>(result.Value);
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var produit =await _manager.GetByIdAsync(id);

        if (produit.Value == null)
            return NotFound();

        await _manager.DeleteAsync(produit.Value);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetAll()
    {
        var produits = await _manager.GetAllAsync();

        if (!produits.Value.Any())
            return NoContent();

        var dtoList = _mapper.Map<IEnumerable<ProduitDto>>(produits.Value);
        return Ok(dtoList);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProduitDetailDto>> Create([FromBody] ProduitDetailDto dto)
    {
        Marque marque = null;
        TypeProduit type = null;

        if (_context != null)
        {
            marque = await _context.Marques
                .FirstOrDefaultAsync(m => m.NomMarque == dto.Marque);
            if (marque == null)
            {
                marque = new Marque { NomMarque = dto.Marque };
                _context.Marques.Add(marque);
                await _context.SaveChangesAsync();
            }

            type = await _context.TypeProduits
                .FirstOrDefaultAsync(t => t.NomTypeProduit == dto.Type);
            if (type == null)
            {
                type = new TypeProduit { NomTypeProduit = dto.Type };
                _context.TypeProduits.Add(type);
                await _context.SaveChangesAsync();
            }
        }
        var produit = _mapper.Map<Produit>(dto);

        if (marque != null) produit.IdMarque = marque.IdMarque;
        if (type != null) produit.IdTypeProduit = type.IdTypeProduit;

        await _manager.AddAsync(produit);

        var createdDto = _mapper.Map<ProduitDetailDto>(produit);

        return CreatedAtAction(nameof(Get), new { id = produit.IdProduit }, createdDto);
    }



    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int id, [FromBody] ProduitDetailDto dto)
    {
        var produit = await _manager.GetByIdAsync(id);
        if (produit.Value == null)
            return NotFound();

        // Vérifier/Créer la Marque
        var marque = await _context.Marques
            .FirstOrDefaultAsync(m => m.NomMarque == dto.Marque);
        if (marque == null)
        {
            marque = new Marque { NomMarque = dto.Marque };
            _context.Marques.Add(marque);
            await _context.SaveChangesAsync();
        }

        // Vérifier/Créer le Type
        var type = await _context.TypeProduits
            .FirstOrDefaultAsync(t => t.NomTypeProduit == dto.Type);
        if (type == null)
        {
            type = new TypeProduit { NomTypeProduit = dto.Type };
            _context.TypeProduits.Add(type);
            await _context.SaveChangesAsync();
        }

        // Mise à jour des champs
        produit.Value.NomProduit = dto.NomProduit;
        produit.Value.Description = dto.Description;
        produit.Value.IdMarque = marque.IdMarque;
        produit.Value.IdTypeProduit = type.IdTypeProduit;
        produit.Value.NomPhoto = dto.NomPhoto;
        produit.Value.UriPhoto = dto.UriPhoto;
        produit.Value.StockReel = dto.StockReel;
        produit.Value.StockMin = dto.StockMin;
        produit.Value.StockMax = dto.StockMax;

        await _manager.UpdateAsync(produit.Value, produit.Value);

        return NoContent();
    }

[HttpGet("bytype/{idType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetByType(int idType)
    {
        // Récupération de tous les produits incluant la marque et le type
        var produits = await _manager.GetAllAsync();

        if (produits.Value == null || !produits.Value.Any())
            return NotFound();

        // Filtrer les produits par TypeProduit
        var filtered = produits.Value
            .Where(p => p.TypeProduitNavigation != null && p.TypeProduitNavigation.IdTypeProduit == idType)
            .ToList();

        if (!filtered.Any())
            return NotFound();

        // Map vers ProduitDto
        var dtoList = _mapper.Map<IEnumerable<ProduitDto>>(filtered);

        return Ok(dtoList);
    }
    [HttpGet("{id}/produits")]
    public async Task<ActionResult<IEnumerable<ProduitDto>>> GetProduitsByMarque(int id, [FromServices] IDataRepository<Produit> produitRepo, [FromServices] IMapper mapper)
    {
        var produits = await produitRepo.GetAllAsync();

        // On filtre côté serveur
        var produitsFiltre = produits.Value.Where(p => p.IdMarque == id).ToList();

        if (!produitsFiltre.Any())
            return NotFound();

        var dtoList = mapper.Map<IEnumerable<ProduitDto>>(produitsFiltre);
        return Ok(dtoList);
    }
}
