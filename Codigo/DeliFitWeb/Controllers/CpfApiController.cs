using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Util;

namespace DeliFitWeb.Controllers.Api
{
    [ApiController]
    [Route("api/cpf")]
    public class CpfApiController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public CpfApiController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Valida o formato e os dígitos verificadores de um CPF.
        /// GET /api/cpf/{cpf}/validar/
        /// </summary>
        [HttpGet("{cpf}/validar")]
        public async Task<IActionResult> Validar(string cpf)
        {
            var cpfLimpo = Methods.RemoveNaoNumericos(cpf);
            var valido = Methods.ValidarCpf(cpfLimpo);

            if (!valido)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Verifica se um CPF já está cadastrado na base de dados.
        /// GET /api/cpf/{cpf}
        /// </summary>
        [HttpGet("{cpf}")]
        public async Task<IActionResult> Existe(string cpf)
        {
            var cpfLimpo = Methods.RemoveNaoNumericos(cpf);

            var valido = Methods.ValidarCpf(cpfLimpo);
            if (!valido)
                return BadRequest(new { cpf = cpfLimpo, valido = false, mensagem = "CPF inválido." });

            var cliente = await _clienteService.GetByCpf(cpfLimpo);

            if (cliente == null)
                return NotFound();

            return Ok();
        }
    }
}