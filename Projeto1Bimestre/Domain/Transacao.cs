namespace Projeto1Bimestre.Domain
{
    public class Transacao
    {
        /// <summary>
        /// Id da Transação
        /// </summary>
        public int TransacaoId { get; set; }

        /// <summary>
        ///  Valor da Transação
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Cartão vinculado a Transação
        /// </summary>
        public Cartao Cartao {  get; set; }

        /// <summary>
        /// Código de Segurança do Cartão
        /// </summary>
        public string CVV { get; set; }

        /// <summary>
        /// Quantidade de Parcelas
        /// </summary>
        public int Parcelas { get; set; }

        /// <summary>
        /// Situação da Transação
        /// </summary>
        public TipoSituacaoPagamento Situacao { get; set; }
    }

    public enum TipoSituacaoPagamento
    {
        PENDENTE = 1,
        CONFIRMADO = 2,
        CANCELADO = 3,
        INDEFINIDO = 0
    }
}
