using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class Retorno
    {
        public string msg;
        public string erro;
        public bool status;

        public Retorno(string r_msg = "Processamento Realizado com Sucesso.", string r_erro = "", bool r_status = true)
        {
            this.msg = r_msg;
            this.erro = r_erro;
            this.status = r_status;
        }
    }

    public class RetornoLogin
    {
        public string msg;
        public string erro;
        public bool status;
        public bool status_sessao; // Variavel para informar que ja existe uma sessao ativa em outro navegador ou computador (login/empresa/estabelecimento)

        public RetornoLogin(string r_msg = "Processamento Realizado com Sucesso.", string r_erro = "", bool r_status = true, bool r_status_sessao = false)
        {
            this.msg = r_msg;
            this.erro = r_erro;
            this.status = r_status;
            this.status_sessao = r_status_sessao;   
        }
        public string chave { get; set; }
    }

    public class getVinculoAllRetornoLogin
    {
        public string msg;
        public string erro;
        public bool status;

        public getVinculoAllRetornoLogin(string r_msg = "Processamento Realizado com Sucesso.", string r_erro = "", bool r_status = true)
        {
            this.msg = r_msg;
            this.erro = r_erro;
            this.status = r_status;
        }

        public List<Vinculo> ListaVinculo { get; set; }
    }

    public class getVinculoRetornoLogin
    {
        public string msg;
        public string erro;
        public bool status;

        public getVinculoRetornoLogin(string r_msg = "Processamento Realizado com Sucesso.", string r_erro = "", bool r_status = true)
        {
            this.msg = r_msg;
            this.erro = r_erro;
            this.status = r_status;
        }

        public Object Vinculo { get; set; }
    }

    public class getConsultaCustomizadaRetorno
    {
        public string msg;
        public string erro;
        public bool status;

        public getConsultaCustomizadaRetorno(string r_msg = "Processamento Realizado com Sucesso.", string r_erro = "", bool r_status = true)
        {
            this.msg = r_msg;
            this.erro = r_erro;
            this.status = r_status;
        }
        public Object Consulta { get; set; }
    }

    public class getCustomizacaoRetorno
    {
        public string msg;
        public string erro;
        public bool status;

        public getCustomizacaoRetorno(string r_msg = "Processamento Realizado com Sucesso.", string r_erro = "", bool r_status = true)
        {
            this.msg = r_msg;
            this.erro = r_erro;
            this.status = r_status;
        }
        public List<Dictionary<string, string>> listaConsulta { get; set; }
    }
}