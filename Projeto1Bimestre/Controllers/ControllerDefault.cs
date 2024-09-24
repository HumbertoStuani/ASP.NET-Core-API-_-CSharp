using Microsoft.AspNetCore.Mvc;

namespace Projeto1Bimestre.Controllers
{
    public class ControllerDefault : ControllerBase
    {

        protected ControllerDefault()
        {
            
        }

        protected object APIReturnDefault(bool sucesso, object dados, string mensagem = "")
        {

            return new
            {
                sucesso = sucesso,
                dados = dados,
                mensagem = mensagem
            };

        }

    }
}
