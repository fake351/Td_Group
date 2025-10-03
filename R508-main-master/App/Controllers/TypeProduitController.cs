using App.Models.Repository;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using App.DTO;

namespace App.Controllers
{
    [Route("api/types-produits")]
    [ApiController]
    public class TypeProduitController : ControllerBase
    {
        private readonly IDataRepository<TypeProduit> _manager;
        private readonly IMapper _mapper;

        public TypeProduitController(IDataRepository<TypeProduit> manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        // GET: api/types-produits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypeProduitDto>>> GetAll()
        {
            var types = await _manager.GetAllAsync();

            if (!types.Value.Any())
                return NoContent();

            var typesDto = _mapper.Map<IEnumerable<TypeProduitDto>>(types.Value);
            return Ok(typesDto);
        }

        // GET: api/types-produits/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeProduitDto>> Get(int id)
        {
            var typeProduit = await _manager.GetByIdAsync(id);

            if (typeProduit.Value == null)
                return NotFound();

            var dto = _mapper.Map<TypeProduitDto>(typeProduit.Value);
            return Ok(dto);
        }

        // POST: api/types-produits
        [HttpPost]
        public async Task<ActionResult<TypeProduitDto>> Post([FromBody] TypeProduitDto typeProduitDto)
        {
            var entity = _mapper.Map<TypeProduit>(typeProduitDto);

            await _manager.AddAsync(entity);

            var dto = _mapper.Map<TypeProduitDto>(entity);

            return CreatedAtAction(nameof(Get), new { id = entity.IdTypeProduit }, dto);
        }

        // PUT: api/types-produits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TypeProduitDto typeProduitDto)
        {
            var existing = await _manager.GetByIdAsync(id);

            if (existing.Value == null)
                return NotFound();

            var updatedEntity = _mapper.Map<TypeProduit>(typeProduitDto);

            await _manager.UpdateAsync(existing.Value, updatedEntity);

            return NoContent();
        }

        // DELETE: api/types-produits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var typeProduit = await _manager.GetByIdAsync(id);

            if (typeProduit.Value == null)
                return NotFound();

            await _manager.DeleteAsync(typeProduit.Value);
            return NoContent();
        }
    }
}
