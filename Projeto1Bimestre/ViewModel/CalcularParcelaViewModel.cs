using System.ComponentModel.DataAnnotations;

namespace Projeto1Bimestre.ViewModel
{
    public class CalcularParcelaViewModel
    {
        /// <summary>
        /// Valor Total da Transação
        /// </summary>
        [Required]
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// Taxa de Juros aplicada
        /// </summary>
        [Required]
        public decimal TaxaJuros { get; set; }

        /// <summary>
        /// Quantidade de parcelas
        /// </summary>
        [Required]
        public int Parcelas {  get; set; }

    }
}
