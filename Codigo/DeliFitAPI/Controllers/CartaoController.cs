using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly ICartaoService _service;

        public CartaoController(ICartaoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Cartao>> GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<Cartao> Get(uint id)
        {
            var cartao = _service.Get(id);
            if (cartao == null) return NotFound();
            return Ok(cartao);
        }

        [HttpGet("cliente/{idCliente}")]
        public ActionResult<IEnumerable<Cartao>> GetByCliente(uint idCliente)
        {
            return Ok(_service.GetByCliente(idCliente));
        }

        [HttpPost]
        public ActionResult<uint> Create([FromBody] Cartao cartao)
        {
            var id = _service.Create(cartao);
            return CreatedAtAction(nameof(Get), new { id = id }, id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(uint id)
        {
            var existing = _service.Get(id);
            if (existing == null) return NotFound();
            _service.Delete(id);
            return NoContent();
        }
    }
}