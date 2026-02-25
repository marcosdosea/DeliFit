using AutoMapper;
using Core;
using Core.Service;
using DeliFitAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeliFitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly ICartaoService _cartaoService;
        private readonly IMapper _mapper;

        public CartaoController(ICartaoService cartaoService, IMapper mapper)
        {
            _cartaoService = cartaoService;
            _mapper = mapper;
        }

        // GET: api/Cartao
        [HttpGet]
        public ActionResult Get()
        {
            var listaCartoes = _cartaoService.GetAll();
            if (listaCartoes == null || !listaCartoes.Any())
                return NotFound();

            return Ok(listaCartoes);
        }

        // GET api/Cartao/5
        [HttpGet("{id}")]
        public ActionResult Get(uint id)
        {
            var cartao = _cartaoService.Get(id);
            if (cartao == null)
                return NotFound();

            return Ok(cartao);
        }

        // GET api/Cartao/cliente/5
        [HttpGet("cliente/{idCliente}")]
        public ActionResult GetByCliente(uint idCliente)
        {
            var cartoes = _cartaoService.GetByCliente(idCliente);
            if (cartoes == null || !cartoes.Any())
                return NotFound();

            return Ok(cartoes);
        }

        // POST api/Cartao
        [HttpPost]
        public ActionResult Post([FromBody] CartaoViewModel cartaoModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dados inválidos");
            }

            var cartao = _mapper.Map<Cartao>(cartaoModel);
            _cartaoService.Create(cartao);

            return Ok(cartao);
        }

        // DELETE api/Cartao/5
        [HttpDelete("{id}")]
        public ActionResult Delete(uint id)
        {
            var cartao = _cartaoService.Get(id);
            if (cartao == null)
                return NotFound();

            _cartaoService.Delete(id);
            return Ok();
        }
    }
}