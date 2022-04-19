using System;
using Microsoft;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using API.Common;

namespace API.Controllers
{
    public class LoginController : Controller
    {
        char[] charsToTrim = { '*', ' ', '\'' };
        Retorno retorno = new Retorno();
        RetornoLogin retornoLogin = new RetornoLogin();
        DB conn = new DB();

        [HttpPost]
        public JsonResult getAuthorizeUser([FromBody] Login l)
        {
            Loginacx loginacx = new Loginacx();
            bool status_sessao_ativa = false;
            try
            {
                if (conn.Open())
                {
                    if (String.IsNullOrEmpty(l.login))
                    {
                        throw new Exception("Login Obrigatório. Favor preencher");
                    }

                    if (String.IsNullOrEmpty(l.senha))
                    {
                        throw new Exception("Senha Obrigatório. Favor preencher");
                    }

                    if (l.empresa == 0)
                    {
                        throw new Exception("Empresa Obrigatório. Favor selecionar");
                    }

                    if (l.estabelecimento == 0)
                    {
                        throw new Exception("Estabelecimento Obrigatório. Favor selecionar");
                    }

                    loginacx.Senha = l.senha.Trim(charsToTrim);
                    loginacx.Usuario = l.login.Trim(charsToTrim);
                    loginacx.CodEstabelecimento = l.estabelecimento;
                    loginacx.CodEmpresa = l.empresa;

                    string query = loginacx.getQueryDadosUsuario();
                    conn.Query(query);

                    Loginacx.Sessao s = new Loginacx.Sessao();

                    s.CodEstabelecimento = l.estabelecimento;
                    s.CodEmpresa = l.empresa;
                    s.CodUsuario = int.Parse(conn.getValueByName("cod_usuario"));

                    l.nom_usuario = conn.getValueByName("nom_usuario");
                    l.nom_empresa = conn.getValueByName("nom_empresa");
                    l.nom_estabelecimento = conn.getValueByName("nom_estabelecimento");

                    status_sessao_ativa = s.validaSessaoAtiva();
                    retornoLogin.status_sessao = status_sessao_ativa;
                    if (!status_sessao_ativa)
                    {
                        if (conn.getRows() == 0)
                        {
                            throw new Exception("Usuário ou Senha inválidos");
                        }
                        else
                        {
                            //Chave c = new Chave();
                            //c.getKey();
                            Acesso a = new Acesso();
                            a.cod_usuario = int.Parse(conn.getValueByName("cod_usuario"));
                            a.cod_empresa = int.Parse(conn.getValueByName("cod_empresa"));
                            a.cod_estabelecimento = int.Parse(conn.getValueByName("cod_estabelecimento"));
                            a.token = API.Commom.TokenService.GenerateToken(l);

                            setControleAcessos(a);

                            l.senha = "********";
                            retornoLogin.token = a.token;
                            retornoLogin.user = l; 
                        }
                    }
                    else
                    {
                        throw new Exception("Usuário ativo em outra sessão. Deseja encerrar as sessões ativas? ");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                retornoLogin = new RetornoLogin("", e.Message, false, status_sessao_ativa);
            }

            conn.Close();
            return Json(retornoLogin);
        }

        [HttpPost]
        public JsonResult getOptionLogin([FromBody] Login l)
        {
            Loginacx.getVinculo login = new Loginacx.getVinculo();
            getVinculoRetornoLogin retorno = new getVinculoRetornoLogin();
            try
            {
                login.Login = l.login;
                login.Senha = l.senha;
                login.StatusAtivo = "S";

                var lista = login.getList();

                retorno.Vinculo = lista;

            }
            catch (Exception e)
            {
                retorno = new getVinculoRetornoLogin("", e.Message, false);
            }

            conn.Close();
            return Json(retorno);
        }

        [HttpGet]
        public JsonResult verificaSessaoAtiva(string token)
        {
            if (String.IsNullOrEmpty(token))
            {
                throw new Exception("Não foi possivel validar Token. Você será redirecionado para a tela de login.");
            }
            token = token.Replace("Bearer ", "");

            retornoLogin = new RetornoLogin("Sua sessão experiou. Você será encaminhado para a tela de login.", "", true, false);
            Loginacx loginacx = new Loginacx();
            bool status_sessao_ativa = false;
            try
            {
                Loginacx.Sessao s = new Loginacx.Sessao();
                s.Token = token;

                status_sessao_ativa = s.validaSessaoAtiva();
                retornoLogin.status_sessao = status_sessao_ativa;
                if (status_sessao_ativa)
                {
                    loginacx.Token = token;

                    string query = loginacx.getQueryRemoveSessaoAtiva();
                    if (!conn.Execute(query))
                    {
                        throw new Exception("Sua sessão expirou. Tivemos alguma falha ao direcionar para a tela de Login. Favor efetuar o logoff e login novamente");
                    }
                    //retornoLogin = new RetornoLogin("Usuário ativo em outra sessão. Deseja encerrar as sessões ativas? ", "", true, status_sessao_ativa);
                }
            }
            catch (Exception e)
            {
                retornoLogin = new RetornoLogin("", e.Message, false, status_sessao_ativa);
            }


            return Json(retornoLogin);
        }

        [HttpPost]

        public JsonResult encerraSessoesAtivasEmpEstab([FromBody] Login l)
        {
            retorno = new Retorno("Acesso liberado. Favor tentar efetuar o login novamente.", "", true);
            Loginacx loginacx = new Loginacx();
            try
            {
                if (conn.Open())
                {
                    loginacx.Usuario = l.login;
                    loginacx.Senha = l.senha;
                    loginacx.CodEmpresa = l.empresa;
                    loginacx.CodEstabelecimento = l.estabelecimento;

                    string query = loginacx.getQueryDadosUsuario();
                    conn.Query(query);

                    if (conn.getRows() > 0)
                    {
                        loginacx.CodUsuario = int.Parse(conn.getValueByName("cod_usuario"));
                        query = loginacx.getQueryControleAcesso();
                        conn.Query(query);

                        if (conn.getRows() > 0)
                        {
                            query = loginacx.getQueryRemoveSessaoAtiva();
                            if (!conn.Execute(query))
                            {
                                throw new Exception("Não foi possivel finalizar as sessões ativas. Favor entrar em contato com o Administrador");
                            }
                        }
                        else
                        {
                            throw new Exception("Não existe sessao ativa para o Usuário.");
                        }
                    }
                    else
                    {
                        throw new Exception("Usuario não cadastrado. Favor entrar em contato com o Administrador");
                    }
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

            conn.Close();
            return Json(retorno);
        }

        [HttpDelete]
        public JsonResult encerraSessoesAtivasToken(string token)
        {
            retorno = new Retorno("Acesso liberado. Favor tentar efetuar o login novamente.", "", true);

            Loginacx loginacx = new Loginacx();
            try
            {
                if (conn.Open())
                {
                    string query = "";

                    loginacx.Token = token;
                    query = loginacx.getQueryControleAcesso();
                    conn.Query(query);

                    if (conn.getRows() > 0)
                    {
                        query = loginacx.getQueryRemoveSessaoAtiva();
                        if (!conn.Execute(query))
                        {
                            throw new Exception("Não foi possivel finalizar as sessões ativas. Favor entrar em contato com o Administrador");
                        }
                    }
                    else
                    {
                        throw new Exception("Não existe sessao ativa para o Usuário.");
                    }
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
            conn.Close();
            return Json(retorno);
        }

        public void setControleAcessos(Acesso a){
            DB connLogin = new DB();    
            
            if (connLogin.Open())
            {
                var query = $"select * from acx_controle_acessos " +
                            $"  where cod_usuario = {a.cod_usuario} and cod_empresa = {a.cod_empresa} and cod_estabelecimento = {a.cod_estabelecimento}";
                connLogin.Query(query);

                connLogin.Begin();
                if (connLogin.getRows() > 0)
                {
                    query = $"delete from acx_controle_acessos " +
                            $"  where cod_usuario = {a.cod_usuario} and cod_empresa = {a.cod_empresa} and cod_estabelecimento = {a.cod_estabelecimento}";
                    if (!connLogin.Execute(query))
                    {
                        connLogin.Rollback();
                        throw new Exception($"Falha ao gerar Chave de acesso. Não foi possivel efetuar o Login. {connLogin.ErrorMsg} ");
                    }
                }
                
                query = $"insert into acx_controle_acessos(cod_usuario, cod_empresa, cod_estabelecimento, chave_acesso) " +
                            $" values({a.cod_usuario}, {a.cod_empresa}, {a.cod_estabelecimento}, '{a.token}')";
                if (!connLogin.Execute(query))
                {
                    connLogin.Rollback();
                    throw new Exception($"Falha ao gerar Chave de acesso. Não foi possivel efetuar o Login. {connLogin.ErrorMsg} ");
                }
                connLogin.Commit();
            }
            else
            {
                throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
            }
        }
    }
}
