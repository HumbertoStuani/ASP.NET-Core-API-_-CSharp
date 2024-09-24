using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projeto1Bimestre.Domain;

namespace Projeto1Bimestre.Controllers
{
    //[Authorize("APIAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartoesController : ControllerBase
    {

        private readonly Services.CartaoService _cartaoService;

        public CartoesController(Services.CartaoService cartaoService)
        {
            _cartaoService = cartaoService;
        }

        /// <summary>
        /// •	Este endpoint recebe o número do cartão de crédito e retorna sua bandeira (VISA, MASTERCARD, ELO...) de acordo com a regra de negócio fictícia dada a seguir, que considera os primeiros 4 dígitos do número e o 8º do cartão (BIN): 
        /// </summary>
        /// <param name="cartao"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{cartao}/obter-bandeira")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ObterBandeira (string cartao)
        {
            string ret = _cartaoService.ObterBandeiraCartao(cartao);
            Console.WriteLine(ret);
            if (ret != "")
                return StatusCode(200,ret);
            return StatusCode(404);
        }


        /// <summary>
        /// •	Este endpoint recebe o número do cartão de crédito e retorna um valor booleano indicando se o cartão é válido. Isso deve ser feito verificando sua existência e validade do cartão na tabela "CARTAO".
        /// </summary>
        /// <param name="cartao"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{cartao}/valido")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ObterValidade(string cartao)
        {
            bool ret = _cartaoService.ObterValidadeCartao(cartao);
            return Ok(ret);
        }

    }
}
