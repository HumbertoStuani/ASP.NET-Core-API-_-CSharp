using Projeto1Bimestre.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Projeto1Bimestre.Services
{
    public class PagamentoService
    {
        private readonly BD _bd;
        private readonly ILogger<CartaoService> _logger;
        private readonly CartaoService _cartaoService;

        public PagamentoService(BD bd, ILogger<CartaoService> logger, CartaoService cartaoService)
        {
            _bd = bd;
            _logger = logger;
            _cartaoService = cartaoService;
        }

        public TipoSituacaoPagamento obterSituacaoTransacao (int id)
        {
            TipoSituacaoPagamento codSituacao = TipoSituacaoPagamento.INDEFINIDO;

            try
            {
                var conexao = _bd.CriarConexao();
                try
                {
                    conexao.Open();

                    var cmd = conexao.CreateCommand();

                    // Primeira query: Verifica se o cartão é válido
                    cmd.CommandText = "select * from Transacao where TransacaoId = @idPagamento";
                    cmd.Parameters.AddWithValue("idPagamento", id);
                    var result = cmd.ExecuteReader();

                    // Verifica se há registros válidos
                    if (result.Read()) // Retorna true se há uma linha
                        codSituacao = (TipoSituacaoPagamento)Convert.ToInt32(result["Situacao"]);
                    else
                    {
                        _logger.LogInformation("Transação não encontrada");
                        throw new Exception("Transação não encontrada");
                    }   
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro na consulta de transação");
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conexao.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer conexão");
                throw new Exception(ex.Message);
            }

            return codSituacao;
        }


        public List<object> CalcularParcelas(dynamic param)
        {
            List<object> list = new List<object>();
            object aux;
            decimal valor = param.ValorTotal * param.TaxaJuros;
            
            for(int i=1; i<= param.QtdeParcelas; i++)
            {
                aux = new
                {
                    parcela = i,
                    valor = valor / param.QtdeParcelas
                };
                list.Add(aux);
            }

            return list;
        }

        public int CriarPagamento (dynamic param)
        {
            int ret = -1;
            try
            {
                var conexao = _bd.CriarConexao();
                conexao.Open();
                MySql.Data.MySqlClient.MySqlTransaction transacao = conexao.BeginTransaction();

                try
                {
                    if(_cartaoService.ObterValidadeCartao(param.NumeroCartao) == true)
                    {
                        var cmd = conexao.CreateCommand();
                        cmd.Parameters.AddWithValue("@Valor", param.Valor);
                        cmd.Parameters.AddWithValue("@Cartao", param.NumeroCartao);
                        cmd.Parameters.AddWithValue("@CVV", param.CVV);
                        cmd.Parameters.AddWithValue("@Parcelas", param.QtdeParcelas);
                        cmd.Parameters.AddWithValue("@Situacao", 1);

                        cmd.CommandText = "insert into Transacao (Valor, Cartao, CVV, Parcelas, Situacao) values (@Valor,@Cartao,@CVV,@Parcelas,@Situacao)";
                        cmd.ExecuteNonQuery();

                        ret = (int)cmd.LastInsertedId;

                        transacao.Commit();
                    }
                    else
                    {
                        _logger.LogInformation("Cartão inválido");
                    }
                }
                catch (Exception e)
                {
                    transacao.Rollback();
                    _logger.LogError("Erro no pagamento");
                }
                finally
                {
                    conexao.Close();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Erro ao realizar conexao");
                throw new Exception("Erro ao realizar conexao");
            }

            return ret;
        }

        public bool CancelarPagamento (int id)
        {
            bool ret = false;

            try
            {
                var conexao = _bd.CriarConexao();
                conexao.Open();

                try
                {
                    TipoSituacaoPagamento situacao = obterSituacaoTransacao(id);
                    if (situacao != TipoSituacaoPagamento.CONFIRMADO)
                    {
                        var cmd = conexao.CreateCommand();
                        cmd.CommandText = "update Transacao set Situacao = 3 where TransacaoId = @Id";
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                        ret = true;
                    }
                    else
                    {
                        _logger.LogInformation("Transacao não pode ser cancelada, visto que já está ");
                        throw new Exception("Transacao não pode ser cancelada");
                    }
                    
                }
                catch
                {
                    _logger.LogInformation("Erro no cancelamento de pagamento");
                }
                finally
                {
                    conexao.Close();
                }

            }
            catch (Exception e) {
                _logger.LogError(e,"Erro ao realizar conexao");
                throw new Exception("Erro ao realizar conexao: "+e);
            }

            return ret;
        }


        public bool ConfirmarPagamento(int id)
        {
            bool ret = false;

            try
            {
                var conexao = _bd.CriarConexao();
                conexao.Open();

                try
                {
                    TipoSituacaoPagamento situacao = obterSituacaoTransacao(id);
                    if (situacao != TipoSituacaoPagamento.CANCELADO)
                    {
                        var cmd = conexao.CreateCommand();
                        cmd.CommandText = "update Transacao set Situacao = 2 where TransacaoId = @Id";
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                        ret = true;
                    }
                    else
                    {
                        _logger.LogInformation("Transacao não pode ser confirmada, visto que já está "+ situacao.ToString());
                        throw new Exception("Transacao não pode ser confirmada");
                    }

                }
                catch
                {
                    _logger.LogInformation("Erro na confirmação de pagamento");
                }
                finally
                {
                    conexao.Close();
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao realizar conexao");
                throw new Exception("Erro ao realizar conexao: " + e);
            }

            return ret;
        }
    }
}
