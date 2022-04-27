using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class Retorno
    {
        public string msg { get; set; }
        public string erro { get; set; }
        public bool status { get; set; }

        public Retorno(string r_msg = "Processamento Realizado com Sucesso.", string r_erro = "", bool r_status = true)
        {
            this.msg = r_msg;
            this.erro = r_erro;
            this.status = r_status;
        }
    }

    public class RetornoLogin : Retorno
    {
        public bool status_sessao { get; set; } // Variavel para informar que ja existe uma sessao ativa em outro navegador ou computador (login/empresa/estabelecimento)

        public string token { get; set; }
        public Object user { get; set; }   
    }

    public class getVinculoAllRetornoLogin : Retorno
    {
        public List<Vinculo> ListaVinculo { get; set; }
    }

    public class getVinculoRetornoLogin : Retorno
    {
        public Object Vinculo { get; set; }
    }

    public class getConsultaCustomizadaRetorno : Retorno
    {
        public Object Consulta { get; set; }
    }

    public class getCustomizacaoRetorno : Retorno
    {
        public List<Dictionary<string, string>> listaConsulta { get; set; }
    }

    public class getAllEmpresaRetorno : Retorno
    {
        public List<Dictionary<string, string>> listaEmpresa { get; set; }
    }
}