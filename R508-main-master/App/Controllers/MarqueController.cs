using App.DTO;
using App.Models;
using App.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Route("api/marques")]
    [ApiController]
    public class MarqueController : ControllerBase
    {
        private readonly IDataRepository<Marque> _manager;
        private readonly IMapper _mapper;

        public MarqueController(IDataRepository<Marque> manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        // GET api/marques
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarqueDto>>> GetAll()
        {
            var marques = await _manager.GetAllAsync();

            if (!marques.Value.Any())
                return NoContent();

            var dtoList = _mapper.Map<IEnumerable<MarqueDto>>(marques.Value);
            return Ok(dtoList);
        }

        // GET api/marques/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MarqueDto>> Get(int id)
        {
            var marque = await _manager.GetByIdAsync(id);

            if (marque.Value == null)
                return NotFound();

            var dto = _mapper.Map<MarqueDto>(marque.Value);
            return Ok(dto);
        }

        // POST api/marques
        [HttpPost]
        public async Task<ActionResult<MarqueDto>> Post([FromBody] MarqueDto dto)
        {
            var marque = _mapper.Map<Marque>(dto);
            await _manager.AddAsync(marque);

            var createdDto = _mapper.Map<MarqueDto>(marque);

            return CreatedAtAction(nameof(Get), new { id = marque.IdMarque }, createdDto);
        }

        // PUT api/marques/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] MarqueDto dto)
        {
            var existing = await _manager.GetByIdAsync(id);

            if (existing.Value == null)
                return NotFound();

            var updatedEntity = _mapper.Map(dto, existing.Value);
            await _manager.UpdateAsync(existing.Value, updatedEntity);

            return NoContent();
        }

        // DELETE api/marques/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var marque = await _manager.GetByIdAsync(id);

            if (marque.Value == null)
                return NotFound();

            await _manager.DeleteAsync(marque.Value);
            return NoContent();
        }
    }
}
