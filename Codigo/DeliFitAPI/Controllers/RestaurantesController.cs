using AutoMapper;
using Core;
using Core.Identity.Data;
using Core.Service;
using DeliFitAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DeliFitAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class RestaurantesController : ControllerBase
{
    private readonly IRestauranteService _restauranteService;
    private readonly IMapper _mapper;
    private readonly UserManager<UsuarioIdentity>? _userManager;
    private readonly RoleManager<IdentityRole>? _roleManager;
    private readonly IEmailSender? _emailSender;

    public RestaurantesController(IRestauranteService restauranteService, IMapper mapper, UserManager<UsuarioIdentity>? userManager = null, RoleManager<IdentityRole>? roleManager = null, IEmailSender? emailSender = null)
    {
        _restauranteService = restauranteService;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
    }

    // GET: api/<RestaurantesController>
    [HttpGet]
    public ActionResult Get()
    {
        var listaRestaurantes = _restauranteService.GetAll();
        return Ok(listaRestaurantes);
    }

    // GET api/<RestaurantesController>/5
    [HttpGet("{id}")]
    public ActionResult Get(uint id)
    {
        var restaurante = _restauranteService.Get(id);
        if (restaurante == null)
        {
            return NotFound();
        }
        return Ok(restaurante);
    }

    // POST api/<RestaurantesController>
    [HttpPost]
    public ActionResult Post([FromBody] RestauranteViewModel restauranteModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Dados inválidos");
        }
        var restaurante = _mapper.Map<Restaurante>(restauranteModel);
        _restauranteService.Create(restaurante);

        return Ok(restaurante);
    }

    // PUT api/<RestaurantesController>/5
    [HttpPut("{id}")]
    public ActionResult Put(uint id, [FromBody] RestauranteViewModel restauranteModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Dados inválidos");
        }
        var restaurante = _mapper.Map<Restaurante>(restauranteModel);
        _restauranteService.Edit(restaurante);
        return Ok(restaurante);
    }

    // DELETE api/<RestaurantesController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(uint id)
    {
        Restaurante? restaurante = _restauranteService.Get(id);
        if (restaurante == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(restaurante.Email))
        {
            var user = await _userManager.FindByEmailAsync(restaurante.Email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        _restauranteService.Delete(id);
        return Ok();
    }
}
