using System;
using Microsoft;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using API.Models;
using API.Common;

namespace API.Controllers
{
    public class ConsultaCustomizadaController : Controller
    {
        char[] charsToTrim = { '*', ' ', '\'' };
        Retorno retorno = new Retorno();
        getConsultaCustomizadaRetorno retornoConsulta = new getConsultaCustomizadaRetorno();
        DB conn = new DB();

        [HttpPost]
        public JsonResult insert([FromBody] Customizacao consulta, string token)
        {
            try
            {
                if (conn.Open())
                {

                    Loginacx.DadosUsuario t = new Loginacx.DadosUsuario(token);

                    var query = "";
                    ConsultaCustomizada c = new ConsultaCustomizada();
                    c.CodEmpresa = t.CodEmpresa;
                    c.CodUsuario = t.CodUsuario;
                    c.CodEstabelecimento = t.CodEstabelecimento;
                    c.Query = consulta.query;
                    c.Apelido = consulta.apelido_consulta;
                    if (c.validaCustomizacaoExistente())
                    {
                        throw new Exception("Consulta já registrada. Inclusão cancelada");
                    }

                    conn.Begin();
                    query = $"insert into consulta_customizada(cod_usuario, cod_empresa, cod_estabelecimento, apelido_consulta, consulta)" +
                            $" values({t.CodUsuario}, {t.CodEmpresa}, {t.CodEstabelecimento}, '{consulta.apelido_consulta}', '{consulta.query}')";
                    if (!conn.Execute(query))
                    {
                        throw new Exception($"Inclusão cancelada. Não foi possivel continuar com o cadastro. {conn.ErrorMsg}");
                    }
                    conn.Commit();
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                conn.Rollback();
                retorno = new Retorno("", e.Message, false);
            }

            conn.Close();
            return Json(retorno);
        }

        [HttpPut]
        public JsonResult update([FromBody] Customizacao consulta, string token)
        {
            try
            {
                if (conn.Open())
                {
                    Loginacx.DadosUsuario t = new Loginacx.DadosUsuario(token);

                    var query = "";
                    ConsultaCustomizada c = new ConsultaCustomizada();
                    c.CodEmpresa = t.CodEmpresa;
                    c.CodUsuario = t.CodUsuario;
                    c.CodEstabelecimento = t.CodEstabelecimento;
                    c.CodConsulta = consulta.cod_consulta;

                    if (!c.validaCustomizacaoExistente())
                    {
                        throw new Exception("Consulta não cadastrada. Modificação cancelada!");
                    }

                    conn.Begin();
                    query = $"update consulta_customizada set apelido_consulta = '{consulta.apelido_consulta}', consulta = '{consulta.query}' " +
                            $" where cod_usuario = {t.CodUsuario} " +
                            $" and cod_empresa = {t.CodEmpresa} " +
                            $" and cod_estabelecimento = {t.CodEstabelecimento} " +
                            $" and cod_consulta = {consulta.cod_consulta}";
                    if (!conn.Execute(query))
                    {
                        throw new Exception($"Modificação cancelada. Não foi possivel continuar. {conn.ErrorMsg}");
                    }
                    conn.Commit();
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                conn.Rollback();
                retorno = new Retorno("", e.Message, false);
            }

            conn.Close();
            return Json(retorno);
        }

        [HttpGet]
        public JsonResult consulta([FromBody] string token)
        {
            Loginacx.DadosUsuario t = new Loginacx.DadosUsuario(token);

            getCustomizacaoRetorno retornoCustomizacao = new getCustomizacaoRetorno();
            retornoCustomizacao.listaConsulta = new List<Dictionary<string, string>>();
            try
            {
                if (conn.Open())
                {
                    AllConsultas c = new AllConsultas();
                    c.CodEmpresa = t.CodEmpresa;
                    c.CodEstabelecimento = t.CodEstabelecimento;
                    c.CodUsuario = t.CodUsuario;

                    retornoCustomizacao.listaConsulta = c.returnConsulta();
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                retorno = new Retorno("", e.Message, false);
            }
            return Json(retornoCustomizacao);
        }

        [HttpPost]
        public JsonResult consulta([FromBody] int cod_consulta, string token)
        {
            Loginacx.DadosUsuario t = new Loginacx.DadosUsuario(token);

            getCustomizacaoRetorno retornoCustomizacao = new getCustomizacaoRetorno();
            retornoCustomizacao.listaConsulta = new List<Dictionary<string, string>>();
            try
            {
                if (conn.Open())
                {
                    ConsultaCustomizada c = new ConsultaCustomizada();
                    c.CodConsulta = cod_consulta;
                    c.CodEmpresa = t.CodEmpresa;
                    c.CodEstabelecimento = t.CodEstabelecimento;
                    c.CodUsuario = t.CodUsuario;

                    retornoCustomizacao.listaConsulta = c.returnConsulta();
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                retornoCustomizacao = new getCustomizacaoRetorno("", e.Message, false);
            }
            return Json(retornoCustomizacao);
        }

        [HttpDelete]
        public JsonResult delete([FromBody] Customizacao consulta,string token)
        {
            Loginacx.DadosUsuario t = new Loginacx.DadosUsuario(token);

            try
            {
                if (conn.Open())
                {
                    var query = "";
                    ConsultaCustomizada c = new ConsultaCustomizada();
                    c.CodEmpresa = t.CodEmpresa;
                    c.CodUsuario = t.CodUsuario;
                    c.CodEstabelecimento = t.CodEstabelecimento;
                    c.Query = consulta.query;
                    c.Apelido = consulta.apelido_consulta;
                    c.CodConsulta = consulta.cod_consulta;

                    if (!c.validaCustomizacaoExistente())
                    {
                        throw new Exception("Consulta não registrada. Deleção cancelada");
                    }

                    conn.Begin();
                    query = $"delete from consulta_customizada " +
                            $" where cod_usuario = {t.CodUsuario} " +
                            $" and cod_empresa = {t.CodEmpresa} " +
                            $" and cod_estabelecimento = {t.CodEstabelecimento} " +
                            $" and cod_consulta = {consulta.cod_consulta}" +
                            $" and apelido_consulta = '{consulta.apelido_consulta}'" +
                            $" and consulta = '{consulta.query}'";
                    if (!conn.Execute(query))
                    {
                        throw new Exception($"Deleção cancelada. Não foi possivel continuar. {conn.ErrorMsg}");
                    }
                    conn.Commit();

                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                conn.Rollback();
                retorno = new Retorno("", e.Message, false);
            }
            conn.Commit();
            return Json(retorno);
        }
    }
}
