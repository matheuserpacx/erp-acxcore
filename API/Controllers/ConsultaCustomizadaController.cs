using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ConsultaCustomizadaController : Controller
    {
        char[] charsToTrim = { '*', ' ', '\'' };
        Retorno retorno = new Retorno();
        getConsultaCustomizadaRetorno retornoConsulta = new getConsultaCustomizadaRetorno();
        DB conn = new DB();

        [HttpPost]
        [Authorize]
        public JsonResult insert([FromBody] ConsultaInfos consulta)
        {
            try
            {
                if (conn.Open())
                {
                    dynamic emp = User.FindFirst("empresa");
                    dynamic estab = User.FindFirst("estabelecimento");
                    dynamic cod_usu = User.FindFirst("cod_usuario");

                    var query = "";
                    ConsultaCustomizada c = new ConsultaCustomizada();
                    c.CodEmpresa = emp.Value;
                    c.CodUsuario = int.Parse(cod_usu.Value);
                    c.CodEstabelecimento = int.Parse(estab.Value);
                    c.Query = consulta.query;
                    c.Apelido = consulta.apelido_consulta;
                    c.CodWindow = consulta.cod_tela;
                    if (c.validaCustomizacaoExistente())
                    {
                        throw new Exception("Consulta já registrada. Inclusão cancelada");
                    }

                    conn.Begin();
                    query = $"insert into acx_consulta_customizada(cod_usuario, cod_empresa, cod_estabelecimento, cod_window, apelido_consulta, consulta)" +
                            $" values({c.CodUsuario}, {c.CodEmpresa}, {c.CodEstabelecimento}, {c.CodWindow}, '{consulta.apelido_consulta}', '{consulta.query}')";
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
        [Authorize]
        public JsonResult update([FromBody] ConsultaInfos consulta)
        {
            try
            {
                if (conn.Open())
                {
                    dynamic emp = User.FindFirst("empresa");
                    dynamic estab = User.FindFirst("estabelecimento");
                    dynamic cod_usu = User.FindFirst("cod_usuario");

                    var query = "";
                    ConsultaCustomizada c = new ConsultaCustomizada();
                    c.CodEmpresa = emp.Value;
                    c.CodUsuario = int.Parse(cod_usu.Value);
                    c.CodEstabelecimento = int.Parse(estab.Value);
                    c.CodConsulta = consulta.cod_consulta;

                    if (!c.validaCustomizacaoExistente())
                    {
                        throw new Exception("Consulta não cadastrada. Modificação cancelada!");
                    }

                    conn.Begin();
                    query = $"update acx_consulta_customizada set apelido_consulta = '{consulta.apelido_consulta}', consulta = '{consulta.query}' " +
                            $" where cod_usuario = {c.CodUsuario} " +
                            $" and cod_empresa = '{c.CodEmpresa}' " +
                            $" and cod_estabelecimento = {c.CodEstabelecimento} " +
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

        [HttpPost]
        [Authorize]
        public JsonResult consultaAll([FromBody] ConsultaInfos consulta)
        {
            getCustomizacaoRetorno retornoCustomizacao = new getCustomizacaoRetorno();
            retornoCustomizacao.listaConsulta = new List<Dictionary<string, string>>();

            dynamic emp = User.FindFirst("empresa");
            dynamic estab = User.FindFirst("estabelecimento");
            dynamic cod_usu = User.FindFirst("cod_usuario");
            try
            {
                if (conn.Open())
                {
                    AllConsultas c = new AllConsultas();    
                    c.CodEmpresa = emp.Value;
                    c.CodEstabelecimento = int.Parse(estab.Value);
                    c.CodUsuario = int.Parse(cod_usu.Value);
                    c.CodWindow = consulta.cod_tela;

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

        [HttpPost]
        [Authorize]
        public JsonResult consulta([FromBody] ConsultaInfos consulta)
        {
            dynamic emp = User.FindFirst("empresa");
            dynamic estab = User.FindFirst("estabelecimento");
            dynamic cod_usu = User.FindFirst("cod_usuario");

            getCustomizacaoRetorno retornoCustomizacao = new getCustomizacaoRetorno();
            retornoCustomizacao.listaConsulta = new List<Dictionary<string, string>>();
            try
            {
                if (conn.Open())
                {
                    ConsultaCustomizada c = new ConsultaCustomizada();
                    c.CodConsulta = consulta.cod_consulta;
                    c.CodEmpresa = emp.Value;
                    c.CodEstabelecimento = int.Parse(estab.Value);
                    c.CodUsuario = int.Parse(cod_usu.Value);

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
        [Authorize]
        public JsonResult delete([FromBody] ConsultaInfos consulta)
        {
            dynamic emp = User.FindFirst("empresa");
            dynamic estab = User.FindFirst("estabelecimento");
            dynamic cod_usu = User.FindFirst("cod_usuario");

            try
            {
                if (conn.Open())
                {
                    var query = "";
                    ConsultaCustomizada c = new ConsultaCustomizada();
                    c.CodEmpresa = emp.Value;
                    c.CodUsuario = int.Parse(cod_usu.Value);
                    c.CodEstabelecimento = int.Parse(estab.Value);
                    c.Query = consulta.query;
                    c.Apelido = consulta.apelido_consulta;
                    c.CodConsulta = consulta.cod_consulta;

                    if (!c.validaCustomizacaoExistente())
                    {
                        throw new Exception("Consulta não registrada. Deleção cancelada");
                    }

                    conn.Begin();
                    query = $"delete from acx_consulta_customizada " +
                            $" where cod_usuario = {c.CodUsuario} " +
                            $" and cod_empresa = '{c.CodEmpresa}' " +
                            $" and cod_estabelecimento = {c.CodEstabelecimento} " +
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
                return Json(retorno);
            }
            conn.Commit();
            return Json(retorno);
        }
    }
}
