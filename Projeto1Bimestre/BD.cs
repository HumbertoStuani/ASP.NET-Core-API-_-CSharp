using MySql.Data.MySqlClient;

namespace Projeto1Bimestre
{
    public class BD
    {
        public MySql.Data.MySqlClient.MySqlConnection CriarConexao()
        {
            string strCon = Environment.GetEnvironmentVariable("STRING_CONEXAO");
            MySqlConnection conexao = new MySqlConnection(strCon);
            return conexao;
        }
    }
}
