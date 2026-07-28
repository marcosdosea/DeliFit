using AutoMapper;
using Core;
using Core.Identity.Data;
using Core.Service;
using DeliFitAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DeliFitAPI.Controllers;




[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{

    private readonly IClienteService _clienteService;
    private readonly IRestauranteService _restauranteService;
    private readonly IMapper _mapper;
    private readonly UserManager<UsuarioIdentity> _userManager;

    public ClientesController(IClienteService clienteService, IRestauranteService restauranteService, IMapper mapper, UserManager<UsuarioIdentity> userManager)
    {
        _clienteService = clienteService;
        _restauranteService = restauranteService;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: api/<ClientesController>
    [HttpGet]
    public ActionResult Get()
    {
        var listaClientes = _clienteService.GetAll();
        if (listaClientes == null || !listaClientes.Any())
            return NotFound();

        return Ok(listaClientes);
    }

    // GET api/<ClientesController>/5
    [HttpGet("{id}")]
    public ActionResult Get(uint id)
    {
        Cliente cliente = _clienteService.Get(id);
        if (cliente == null)
            return NotFound();
        return Ok(cliente);
    }

    // POST api/<ClientesController>
    [HttpPost]
    public ActionResult Post([FromBody] ClienteViewModel clienteModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Dados inválidos");
        }
        var cliente = _mapper.Map<Cliente>(clienteModel);
        _clienteService.Create(cliente);

        return Ok(cliente);
    }

    // PUT api/<ClientesController>/5
    [HttpPut("{id}")]
    public ActionResult Put(uint id, [FromBody] ClienteViewModel clienteModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Dados inválidos");
        }

        var cliente = _mapper.Map<Cliente>(clienteModel);
        if (cliente == null)
        {
            return NotFound();
        }

        _clienteService.Edit(cliente);

        return Ok(cliente);
    }

    // DELETE api/<ClientesController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(uint id)
    {
        Cliente? cliente = _clienteService.Get(id);
        if (cliente == null)
            return NotFound();

        // Remove o usuário do Identity primeiro (se existir)
        if (!string.IsNullOrEmpty(cliente.Email))
        {
            var user = await _userManager.FindByEmailAsync(cliente.Email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        // Remove o cliente do banco delifit
        _clienteService.Delete(id);
        return Ok();
    }
}
