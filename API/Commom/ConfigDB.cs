using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Config
    {
        class ConfigDB
        {
            public static string nametableversao = "acx_versao";
            public static string ErrorMsg = "";
            public static Dictionary<string, Dictionary<int, string>> Script_table = new Dictionary<string, Dictionary<int, string>>();
            public static Dictionary<int, string> dic_table = new Dictionary<int, string>();

            public static void getTables(string versao)
            {
                dic_table = new Dictionary<int, string>();
                int idx = 0;

                switch (versao)
                {
                    case "01.000":
                        dic_table.Add((idx++), "acx_versao");
                        dic_table.Add((idx++), "acx_usuarios");
                        dic_table.Add((idx++), "acx_estabelecimento");
                        dic_table.Add((idx++), "acx_empresa");
                        dic_table.Add((idx++), "acx_vinculo_emp_estab");

                        Script_table.Add(versao, dic_table);
                        break;

                    case "01.001":
                        dic_table.Add((idx++), "acx_consulta_customizada");
                        Script_table.Add(versao, dic_table);
                        break;

                    case "01.002":
                        dic_table.Add((idx++), "acx_controle_acessos");
                        Script_table.Add(versao, dic_table);
                        break;
                }
            }

            public static Dictionary<string, Dictionary<int, string>> getCommandos()
            {
                var Script = new Dictionary<string, Dictionary<int, string>>();
                var dic = new Dictionary<int, string>();

                Script_table.Clear();
                dic_table.Clear();

                int idx = 0;
                int ver = 0;
                //VERSAO 01.000
                dic.Add((idx++), "create table acx_versao(cod_versao [KEY],versao_atual varchar(02),release_atual varchar(03),versao_ant varchar(02),release_ant varchar(03),primary key(cod_versao))");
                dic.Add((idx++), "create table acx_usuarios (cod_login serial,login varchar(20),senha varchar(30),	status_ativo varchar(1),primary key (cod_login, login))");
                dic.Add((idx++), "create table acx_estabelecimento(cod_estab serial,cod_empresa int ,nom_estabelecimento varchar(100),primary key(cod_estab))");
                dic.Add((idx++), "create table acx_empresa(cod_empresa [KEY],nom_empresa varchar(100),primary key(cod_empresa))");
                dic.Add((idx++), "create table acx_vinculo_usuario_emp(cod_vinculo serial,cod_usuario int,cod_empresa int,	primary key(cod_vinculo))");
                var versao = $"01.{(ver++).ToString().PadLeft(3, '0')}";
                Script.Add(versao, dic);
                getTables(versao);

                //VERSAO 01.001
                dic = new Dictionary<int, string>();
                idx = 0;

                dic.Add((idx++), "create table acx_consulta_customizada(cod_consulta [KEY],cod_usuario int,cod_empresa int,cod_estabelecimento int, apelido_consulta varchar(100),consulta varchar(30000))");
                versao = $"01.{(ver++).ToString().PadLeft(3, '0')}";
                Script.Add(versao, dic);
                getTables(versao);

                dic = new Dictionary<int, string>();
                idx = 0;

                dic.Add((idx++), "create table acx_controle_acessos(cod_acesso [KEY],cod_usuario int,cod_empresa int,cod_estabelecimento int,chave_acesso varchar(200), primary key (cod_acesso, cod_usuario, cod_empresa, cod_estabelecimento))");
                versao = $"01.{(ver++).ToString().PadLeft(3, '0')}";
                Script.Add(versao, dic);
                getTables(versao);

                //VERSAO 01.002
                dic = new Dictionary<int, string>();
                idx = 0;

                dic.Add((idx++), "alter table acx_consulta_customizada add column cod_window int, " +
                                 "ADD CONSTRAINT acx_consulta_customizada_pk PRIMARY KEY (cod_consulta, cod_usuario, cod_empresa, cod_estabelecimento, cod_window)");
                versao = $"01.{(ver++).ToString().PadLeft(3, '0')}";
                Script.Add(versao, dic);

            return Script;
            }

            public static void Configurar()
            {
                var arqlog = DateTime.Now.ToString("yyyyMMdd") + "_ConfigBD.log";
                var log = "";
                var gravar = false;

                DB conn = new DB();
                if (conn.Open())
                {
                    try
                    {
                        var sel = $"select versao_atual, release_atual from {nametableversao}";
                        conn.Query(sel);

                        var versao_ant = (String.IsNullOrEmpty(conn.getValueByName("versao_atual").ToString())) ? "000" : conn.addZeroes(int.Parse(conn.getValueByName("versao_atual")), 3);
                        var release_ant = (String.IsNullOrEmpty(conn.getValueByName("release_atual").ToString())) ? "000" : conn.addZeroes(int.Parse(conn.getValueByName("release_atual")), 3);
                        var upd = $"UPDATE {nametableversao} SET versao_ant = {versao_ant}, release_ant = {release_ant} WHERE 1=1";
                        conn.Execute(upd);

                        log += "ConfigBD: Inicio " + Environment.NewLine;
                        var Script = getCommandos();

                        var query = $"SELECT * FROM {nametableversao} ";
                        conn.Query(query);

                        int versao_atual = 0;
                        if (conn.getValueByName("versao_atual") != string.Empty)
                        {
                            versao_atual = Convert.ToInt32(conn.getValueByName("versao_atual"));
                        }

                        log += "ConfigBD: Versao Atual: " + versao_atual + Environment.NewLine;

                        int release_atual = 0;
                        if (conn.getValueByName("release_atual") != string.Empty)
                        {
                            release_atual = Convert.ToInt32(conn.getValueByName("release_atual"));
                        }

                        log += "ConfigBD: Release Atual: " + release_atual + Environment.NewLine;

                        foreach (var Master in Script)
                        {
                            var split = Master.Key.Split('.');
                            int versao = Convert.ToInt32(split[0]);
                            int release = Convert.ToInt32(split[1]);

                            int count_obj = 0;
                            if (Script_table.ContainsKey(Master.Key))
                            {
                                count_obj = Script_table[Master.Key].Count() - 1;
                            }

                            log += "ConfigBD: Verifica Versão: " + versao + Environment.NewLine;
                            log += "ConfigBD: Verifica Release: " + release + Environment.NewLine;

                            var executa = false;

                            if (versao > versao_atual)
                            {
                                executa = true;
                            }

                            if (versao == versao_atual)
                            {
                                if (release > release_atual)
                                {
                                    executa = true;
                                }
                            }

                            if (executa)
                            {
                                var erro = "";
                                gravar = true;
                                foreach (var pair in Master.Value)
                                {
                                    log += "ConfigBD: Executa Comando: " + pair.Value + Environment.NewLine;

                                    var comando = pair.Value;
                                    switch (conn.DATABASE)
                                    {
                                        case "POSTGRESQL":
                                            comando = pair.Value.Replace("[KEY]", "SERIAL");
                                            log += "ConfigBD: Executa Comando Convertido Oracle: " + comando + Environment.NewLine;
                                            break;
                                        case "MYSQL":
                                            comando = pair.Value.Replace("[KEY]", "int not null auto_increment");
                                            log += "ConfigBD: Executa Comando Convertido Oracle: " + comando + Environment.NewLine;
                                            break;
                                        default:
                                            break;
                                    }

                                    if (count_obj > 0)
                                    {
                                        if (dic_table.ContainsKey(pair.Key))
                                        {
                                            var tabela = dic_table[pair.Key];
                                            if (conn.verifica_se_tabela_existe(tabela))
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                var view = dic_table[pair.Key];
                                                if (conn.verifica_se_view_existe(view))
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            count_obj = 0;
                                        }
                                    }

                                    if (!(conn.Execute(comando)))
                                    {
                                        erro = conn.ErrorMsg;
                                        log += "ConfigBD: Erro ao Executar Comando: " + erro + Environment.NewLine;
                                        throw new Exception(erro);
                                    }

                                }

                                query = $"SELECT * FROM {nametableversao}";
                                conn.Query(query);
                                if (conn.getRows() == 0)
                                {
                                    query = $"INSERT INTO {nametableversao}(versao_atual,release_atual,versao_ant,release_ant) VALUES ('" + versao + "','" + release + "','','')";
                                }
                                else
                                {
                                    query = $"UPDATE {nametableversao} SET versao_atual = '" + versao.ToString().PadLeft(2, '0') + "', release_atual = '" + release.ToString().PadLeft(3, '0') + "' WHERE 1=1";
                                }

                                if (!(conn.Execute(query)))
                                {
                                    erro = conn.ErrorMsg;
                                    log += "ConfigBD: Erro ao Atualizar bus_versao: " + erro + Environment.NewLine;
                                    throw new Exception(erro);
                                }
                            }
                            else
                            {
                                log += "ConfigBD: Versão/Release ignorada: " + Master.Key + Environment.NewLine;
                            }
                        }
                    }
                    catch (Exception a)
                    {
                        ErrorMsg = a.Message + Environment.NewLine + a.StackTrace;
                        log += "ConfigBD: PROBLEMA: " + ErrorMsg + Environment.NewLine;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                log += "DBCONFIG: Fim" + Environment.NewLine;
                if (gravar)
                {
                    conn.debug(log, arqlog, true);
                }
            }
        }
    }