using Projeto1Bimestre.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Projeto1Bimestre.Services
{
    public class CartaoService
    {
        private readonly BD _bd;
        private readonly ILogger<CartaoService> _logger;

        public CartaoService (BD bd, ILogger<CartaoService> logger)
        {
            _bd = bd;
            _logger = logger;
        }


        public string ObterBandeiraCartao (string numero)
        {
            if(numero != null && numero.Length == 16)
            {
                if (numero[0] == numero[1] && numero[0] == numero[2] && numero[0] == numero[3]) // quartro primeiras casas iguais
                {
                    if(numero[0] == numero[7])
                    {
                        string ret;
                        char op = numero[0];
                        switch(op)
                        {
                            case '1':
                                ret = "VISA";
                                break;

                            case '2':
                                ret = "MASTERCARD";
                                break;

                            case '3':
                                ret = "ELO";
                                break;

                            default:
                                ret = "BANDEIRA NÃO CONHECIDA";
                                break;
                        }

                        // Retorna 200 se for uma bandeira reconhecida
                        if (ret != "BANDEIRA NÃO CONHECIDA")
                        {
                            return ret;
                        }
                    }
                }

            }
            return "";
        }

        public bool ObterValidadeCartao(string numero)
        {
            bool valido = false;
            try
            {
                var conexao = _bd.CriarConexao();

                try
                {
                    conexao.Open();

                    var cmd = conexao.CreateCommand();

                    // Primeira query: Verifica se o cartão é válido
                    cmd.CommandText = "select * from Cartao where Numero = @numeroCartao";
                    cmd.Parameters.AddWithValue("numeroCartao", numero);
                    var result = cmd.ExecuteReader();

                    // Verifica se há registros válidos
                    if (result.Read()) // Retorna true se há uma linha
                    {
                        DateTime data = Convert.ToDateTime(result["Validade"]);
                        if(data > DateTime.Now)
                            valido = true;
                        else
                            _logger.LogInformation("Cartão vencido");
                    }
                    else
                        _logger.LogInformation("Nenhum cartão foi encontrado com o número fornecido.");  
                     
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao obter validade do cartao");
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conexao.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar Conexão");
                throw new Exception(ex.Message);
            }

            return valido;
        }



    }
}
