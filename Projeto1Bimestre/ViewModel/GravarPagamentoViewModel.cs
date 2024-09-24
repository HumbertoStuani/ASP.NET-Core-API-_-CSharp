using System.ComponentModel.DataAnnotations;

namespace Projeto1Bimestre.ViewModel
{
    public class GravarPagamentoViewModel
    {
        /// <summary>
        /// Valor do Pagamento
        /// </summary>
        [Required]
        public decimal Valor {  get; set; }

        /// <summary>
        /// Número do Cartão Vinculado
        /// </summary>
        [Required]
        public string NumeroCartao { get; set; }

        /// <summary>
        /// Código de Segurança do Cartão
        /// </summary>
        [Required]
        public int CVV { get; set; }

        /// <summary>
        /// Quantidade de Parcelas
        /// </summary>
        [Required]
        public int QtdeParcelas { get; set; }
    }
}
