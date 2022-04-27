using System;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class EmpresaController : Controller
    {

        char[] charsToTrim = { '*', ' ', '\'' };
        DB conn = new DB();

        [HttpPost]
        [Authorize]
        public JsonResult GetDadosEmpresa([FromBody]Empresa emp)
        {
            getAllEmpresaRetorno retorno = new getAllEmpresaRetorno();

            retorno.erro = "";
            retorno.msg = "Processamento realizado com sucesso.";
            retorno.status = true;
            try
            {
                if (conn.Open())
                {
                    PrecessoQuery processar = new PrecessoQuery();
                    retorno.listaEmpresa = processar.select(emp);
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                retorno.erro = e.Message;
                retorno.msg = "";
                retorno.status = false;
            }

            conn.Close();
            return Json(retorno);
        }


        [HttpPost]
        //[Authorize]
        public JsonResult InsertDadosEmpresa([FromBody] Empresa emp)
        {
            Retorno retorno = new Retorno();
            bool status = true;

            retorno.erro = "";
            retorno.msg = "Processamento realizado com sucesso.";
            retorno.status = status;
            try
            {
                if (conn.Open())
                {
                    PrecessoQuery processar = new PrecessoQuery();
                    processar.insert(emp);
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                retorno.erro = e.Message;
                retorno.msg = "";
                retorno.status = status;
            }

            conn.Close();
            return Json(retorno);
        }


        [HttpPost]
        [Authorize]
        public JsonResult UpdateDadosEmpresa([FromBody] Empresa emp)
        {
            Retorno retorno = new Retorno();
            bool status = true;

            retorno.erro = "";
            retorno.msg = "Processamento realizado com sucesso.";
            retorno.status = status;
            try
            {
                if (conn.Open())
                {
                    PrecessoQuery processar = new PrecessoQuery();
                    processar.update(emp);
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                retorno.erro = e.Message;
                retorno.msg = "";
                retorno.status = status;
            }

            conn.Close();
            return Json(retorno);
        }
    }
}
