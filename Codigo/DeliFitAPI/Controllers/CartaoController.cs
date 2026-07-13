using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliFitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartaoController : ControllerBase
    {
        private readonly ICartaoService _cartaoService;

        public CartaoController(ICartaoService cartaoService)
        {
            _cartaoService = cartaoService;
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

        // Não há mais um POST de criação de cartão aqui: número/CVV completos não são mais
        // aceitos por este servidor (só um token gerado pelo SDK do Mercado Pago no cliente).
        // O cadastro de cartão acontece hoje pelo DeliFitWeb (CartaoController + Secure Fields).

        // DELETE api/Cartao/5
        [HttpDelete("{id}")]
        public ActionResult Delete(uint id)
        {
            var cartao = _cartaoService.Get(id);
            if (cartao == null)
                return NotFound();

            try
            {
                _cartaoService.Delete(id);
            }
            catch (DbUpdateException)
            {
                return Conflict("Não é possível excluir este cartão pois ele está vinculado a um carrinho/pedido.");
            }

            return Ok();
        }
    }
}
