using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projeto1Bimestre.Domain;
using Projeto1Bimestre.Services;
using Projeto1Bimestre.ViewModel;

namespace Projeto1Bimestre.Controllers
{
    [Authorize("APIAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentosController : ControllerBase
    { 
        private readonly Services.PagamentoService _pagamentoService;

        public PagamentosController (Services.PagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        /// <summary>
        /// •	Este endpoint permite calcular o valor das parcelas de um pagamento com base no valor total da transação, a taxa de juros e o número de parcelas desejado. Para tal, necessário enviar no corpo da requisição, o Valor Total (decimal), Taxa de Juros (decimal) e Quantidade de Parcelas (int). 
        /// </summary>
        /// <param name="calcularParcelaViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("calcular-parcelas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult CalcularParcelas(CalcularParcelaViewModel calcularParcelaViewModel)
        {
            try
            {
                List<object> parcelas = new List<object>();
                var param = new
                {
                    ValorTotal = calcularParcelaViewModel.ValorTotal,
                    TaxaJuros = calcularParcelaViewModel.TaxaJuros,
                    QtdeParcelas = calcularParcelaViewModel.Parcelas

                };
                parcelas = _pagamentoService.CalcularParcelas(param);
                return Ok(parcelas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        /// <summary>
        /// •	Inicia o processo de pagamento, recebendo os detalhes do pagamento no corpo da requisição: (valor [decimal], número do cartão de crédito [string], CVV [int] e quantidade de parcelas [int]).
        /// </summary>
        /// <param name="gravarPagamentoViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult gravarPagamento(GravarPagamentoViewModel gravarPagamentoViewModel)
        {
            int ret;
            try
            {
                var param = new
                {
                    Valor = gravarPagamentoViewModel.Valor,
                    NumeroCartao = gravarPagamentoViewModel.NumeroCartao,
                    CVV = gravarPagamentoViewModel.CVV,
                    QtdeParcelas = gravarPagamentoViewModel.QtdeParcelas
                };
                ret = _pagamentoService.CriarPagamento(param);

                if(ret != -1)
                    return StatusCode(201,ret);
                return BadRequest("Erro ao realizar pagamento");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// •	Consulta a situação de um pagamento com base no seu ID e retorna apenas a situação do pagamento.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/{id}/situacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ObterSituacao(int id)
        {
            try
            {
                return Ok(_pagamentoService.obterSituacaoTransacao(id).ToString());
            }
            catch (Exception ex) { 
            
                return StatusCode(500, ex.Message);
            
            }

        }

        /// <summary>
        /// •	Cancela um pagamento, apenas se ainda não tenha sido confirmado. Troca a situação para "cancelado/3".
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult CancelarPagamento(int id)
        {
            bool ret;
            try
            {
                ret = _pagamentoService.CancelarPagamento(id);

                if (ret == true)
                    return StatusCode(200, "Pagamento cancelado");
                return BadRequest("Erro ao cancelar pagamento");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// •	Confirma o pagamento do ID informado, trocando a situação para "confirmado/2".
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}/confirmar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult ConfirmarPagamento(int id)
        {
            bool ret;
            try
            {
                ret = _pagamentoService.ConfirmarPagamento(id);

                if (ret == true)
                    return StatusCode(200, "Pagamento confirmado");
                return BadRequest("Erro ao confirmar pagamento");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
