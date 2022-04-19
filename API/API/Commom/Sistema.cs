using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Web;
using API.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace API
{
    class Sistema
    {
        public static bool ReadConfig()
        {
            try
            {
                DB Conn = new DB();
                var Diretorio = Sistema.RootPath();
                String config = Diretorio + "Config.ini";
                Array ini = File.ReadAllLines(config);
                String valor = "";
                String campo = "";
                foreach (string file2 in ini)
                {
                    var file = file2;
                    for (int i = 0; i <= file.Length; i++)
                    {
                        if (i < file.Length)
                        {
                            if (file.Substring(i, 1) == "=")
                            {
                                valor = file.Substring(i + 1, (file.Length - (i + 1)));
                                campo = file.Substring(0, i);
                                break;
                            }
                        }
                    }
                    if (campo != "")
                    {
                        if (campo.Substring(0, 1) == "#")
                        {
                            continue;
                        }
                        switch (campo)
                        {
                            case "[PARAMETROS]":
                                break;
                            default:
                                if (Startup.Parametros.ContainsKey(campo))
                                {
                                    Startup.Parametros[campo] = valor;
                                }
                                else
                                {
                                    Startup.Parametros.Add(campo, valor);
                                }
                                Conn.debug("LEITURA Config.ini CRIADO PARAMETRO: " + campo + " = " + valor);
                                break;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string EncodeBase64(string Character)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Character);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception a)
            {
                // Message.Error(a.Message);
                return Character;
            }
        }

        public static string DecodeBase64(string CharEncoded)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(CharEncoded);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception a)
            {
                //    Message.Error(a.Message);
                return CharEncoded;
            }
        }

        public static string RootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory.ToString().Replace("\\", "/");
        }

        public static void Log(Exception e)
        {
            DB Conn = new DB();
            Conn.debug("##################################################################################");
            Conn.debug("################ LOG DE EXCECAO OCORRIDA - STACK DE PROCESSAMENTO ################");
            Conn.debug("##################################################################################");
            Conn.debug(e.Message + Environment.NewLine + e.StackTrace, "", true);
            Conn.debug("##################################################################################");
            Conn.Close();
        }

        public static void Log(string e)
        {
            DB Conn = new DB();
            Conn.debug("############################# LOG DE SISTEMA #####################################");
            Conn.debug(e);
            Conn.Close();
        }

        
        public static void LimpaLogAntigo()
        {
            try
            {
                var Diretorio = Sistema.RootPath() + "/Log/";
                DirectoryInfo dir = new DirectoryInfo(Diretorio);
                var Files = dir.GetFiles("*.log").OrderByDescending(f => f.LastWriteTime);

                var DiretorioReq = Sistema.RootPath() + "/Requisicoes/";
                DirectoryInfo dirreq = new DirectoryInfo(DiretorioReq);
                var FilesReq = dirreq.GetFiles("*.xml").OrderByDescending(f => f.LastWriteTime);

                int QtdDiasArmazLog = -999;
                if (Startup.Parametros["qtd_dias_armaz_log"] != string.Empty)
                {
                    QtdDiasArmazLog = Convert.ToInt32(Startup.Parametros["qtd_dias_armaz_log"]) * (-1);
                }

                QtdDiasArmazLog--;

                Sistema.Log("Quantidade de dias para manter log armazenado: " + QtdDiasArmazLog);

                var DataCalculada = DateTime.Now.AddDays(QtdDiasArmazLog);
                var entrou = false;
                foreach (FileInfo file in Files)
                {
                    if (file.CreationTime < DataCalculada)
                    {
                        entrou = true;
                        Sistema.Log("Log a ser excluido: "+ file.FullName);
                        File.Delete(file.FullName);
                    }
                }
                if (!entrou)
                {
                    Sistema.Log("Nenhum log para excluir");
                }

                entrou = false;
                foreach (FileInfo file in FilesReq)
                {
                    if (file.CreationTime < DataCalculada)
                    {
                        entrou = true;
                        Sistema.Log("Requisicao a ser excluida: " + file.FullName);
                        File.Delete(file.FullName);
                    }
                }
                if (!entrou)
                {
                    Sistema.Log("Nenhuma requisicao para excluir");
                }


                DB Conn = new DB();
                if (Conn.Open())
                {
                    var query = "DELETE FROM acx_logs WHERE data < '"+ DataCalculada.ToString("yyyy-MM-dd") + "'";
                    if (!Conn.Execute(query))
                    {
                        Sistema.Log("Nenhum registro de log para excluir");
                    }

                    query = "DELETE FROM acx_erros WHERE id NOT IN (SELECT id FROM acx_logs)";
                    if (!Conn.Execute(query))
                    {
                        Sistema.Log("Nenhum erro de log para excluir");
                    }
                }
                Conn.Close();

            }
            catch (Exception e)
            {
                Sistema.Log(e);
            }
        }

        

        public static string RetiraEspeciais(string text)
        {
            try
            {
                var newtext = text;
                if (text == null)
                {
                    return text;
                }
                if (text == string.Empty)
                {
                    return text;
                }

                // caracteres não permitidos
                char[] invalidChars = new char[] { '^', '[', '¨', '#', '+', '%',  '&', ']', '=', '~', '\'', '\\', '"', 'º', 'ª', '°', '´', '`', '^', '~', '×', '<', '>'};

                if (newtext.IndexOfAny(invalidChars) >= 0)
                {
                    string[] temp = newtext.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries);
                    newtext = String.Join(" ", temp);
                }
                else
                {
                    newtext = text;
                }

                return newtext.Replace("ç", "c")
                    .Replace("Ç", "C")
                    .Replace("ã", "a")
                    .Replace("Ã", "A")
                    .Replace("á", "a")
                    .Replace("Á", "A")
                    .Replace("à", "a")
                    .Replace("À", "A")
                    .Replace("â", "a")
                    .Replace("Â", "A")
                    .Replace("ä", "a")
                    .Replace("Ä", "A")
                    .Replace("é", "e")
                    .Replace("É", "E")
                    .Replace("è", "e")
                    .Replace("È", "E")
                    .Replace("ê", "e")
                    .Replace("Ê", "E")
                    .Replace("ë", "e")
                    .Replace("Ë", "E")
                    .Replace("í", "i")
                    .Replace("Í", "I")
                    .Replace("ì", "i")
                    .Replace("Ì", "I")
                    .Replace("î", "i")
                    .Replace("Î", "I")
                    .Replace("ï", "i")
                    .Replace("Ï", "I")
                    .Replace("ó", "o")
                    .Replace("Ó", "O")
                    .Replace("ò", "o")
                    .Replace("Ò", "O")
                    .Replace("ô", "o")
                    .Replace("Ô", "O")
                    .Replace("ö", "o")
                    .Replace("Ö", "O")
                    .Replace("õ", "o")
                    .Replace("Õ", "O")
                    .Replace("ú", "u")
                    .Replace("Ú", "U")
                    .Replace("ù", "u")
                    .Replace("Ù", "U")
                    .Replace("û", "u")
                    .Replace("Û", "U")
                    .Replace("ü", "u")
                    .Replace("Ü", "U");
            }
            catch (Exception e)
            {
                return text;
            }
        }

        public static string toLowerCaseTags(string xml)
        {
            return Regex.Replace(
                xml,
                @"<[^<>]+>",
                m => { return m.Value.ToLower(); },
                RegexOptions.Multiline | RegexOptions.Singleline);
        }

        public static bool Like(string toSearch, string toFind)
        {
            toSearch = toSearch.ToUpper();
            toFind = toFind.ToUpper();
            return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
        }
        
    }
}
