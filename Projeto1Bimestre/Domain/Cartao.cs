namespace Projeto1Bimestre.Domain
{
    public class Cartao
    {
        /// <summary>
        /// Numero do Cartao
        /// </summary>
        public string Id {  get; set; }

        /// <summary>
        /// Data de validade do cartão.
        /// </summary>
        public DateTime Validade { get; set; }
    }
}
