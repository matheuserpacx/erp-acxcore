using System;
using System.Collections.Generic;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using API.Models;

namespace API.Controllers
{
    public class FornecedoresController : Controller
    {
        Retorno retorno = new Retorno("Processamento Realizado com Sucesso.", "", true);
        DB conn = new DB();

        [HttpGet]
        public JsonResult GetDados()
        {
            if (conn.Open())
            {

            }

            List<Object> resultado = new List<object>();
            resultado.Add(new
            {
                Nome = "Linha de Código",
                URL = "www.linhadecodigo.com.br"
            });
            resultado.Add(new
            {
                Nome = "DevMedia",
                URL = "www.devmedia.com.br"
            });
            resultado.Add(new
            {
                Nome = "Mr. Bool",
                URL = "www.mrbool.com.br"
            });
            return Json(resultado);
        }
    }
}
