using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnderecosController : ControllerBase
{
    private readonly IEnderecoService _service;

    public EnderecosController(IEnderecoService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Endereco>> GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Endereco> Get(uint id)
    {
        var endereco = _service.Get(id);
        if (endereco == null) return NotFound();
        return Ok(endereco);
    }

    [HttpPost]
    public ActionResult<uint> Create([FromBody] Endereco endereco)
    {
        var id = _service.Create(endereco);
        return CreatedAtAction(nameof(Get), new { id = id }, id);
    }

    [HttpPut("{id}")]
    public IActionResult Update(uint id, [FromBody] Endereco endereco)
    {
        if (id != endereco.Id) return BadRequest();
        var existing = _service.Get(id);
        if (existing == null) return NotFound();
        _service.Edit(endereco);
        return NoContent();
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
