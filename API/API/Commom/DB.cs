using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;
using System.Web;
using System.Globalization;
using System.Threading;

namespace API
{
    public class DB
    {

        public IDbConnection Conn;
        public IDataReader dr;
        public IDbTransaction Transacao;
        public DataTable dt = new DataTable();
        public DataTable par = new DataTable();
        public String DATABASE = "";
        public String DBAlternativo = string.Empty;
        public String ConnString = "";
        public String HOST = "";
        public String USER = "";
        public String PASS = "";
        public String DBNAME = "";
        public String DBPORT = "";
        public String LOCALE = "";
        public String PROTOCOL = "";
        public String SERVER = "";
        public String SID = "";
        public String ERPNAME = "";
        public String ErrorMsg = "";
        public String AssemblyFile = "";
        public String AssemblyInstance = "";

        public bool Open(string id_empresa = null)
        {

            try
            {
                if (!Sistema.ReadConfig())
                {

                }

                try
                {
                    if (this.Status())
                    {
                        return true;
                    }
                }
                catch { }

                if (!(this.ReadConfig()))
                {
                    throw new Exception("Falha ao efetuar leitura das configurações de conexão. ");
                }

                String ConnString = "";

                if (this.DBAlternativo != string.Empty)
                {
                    this.DATABASE = this.DBAlternativo;
                }

                var Diretorio = this.ReadConfig();
                switch (this.DATABASE.ToUpper())
                {
                    case "MYSQL":
                        ConnString = "Server=" + this.HOST + ";Port=" + this.DBPORT + ";Database=" + this.DBNAME + ";Uid=" + this.USER + ";Pwd=" + this.PASS + ";Pooling=True;CharSet=utf8;Connection Timeout=300;Default Command Timeout=300;";
                        AssemblyFile = Diretorio + "MySql.Data.dll";
                        AssemblyInstance = "MySql.Data.MySqlClient.MySqlConnection";
                        break;
                    case "SQLSERVER":
                        ConnString = "Server=" + this.HOST + "," + this.DBPORT + ";Database=" + this.DBNAME + ";User Id=" + this.USER + ";Password=" + this.PASS;
                        AssemblyFile = Diretorio + "System.Data.dll";
                        AssemblyInstance = "System.Data.SqlClient.SqlConnection";
                        break;
                    case "ORACLE":
                        ConnString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + this.HOST + ")(PORT=" + this.DBPORT + "))(CONNECT_DATA=(SERVICE_NAME=" + this.SID + ")));User Id=" + this.USER + ";Password=" + this.PASS + ";Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;Incr Pool Size=5;Decr Pool Size=2";
                        AssemblyFile = Diretorio + "Oracle.ManagedDataAccess.dll";
                        AssemblyInstance = "Oracle.ManagedDataAccess.Client.OracleConnection";
                        break;
                    case "POSTGRESQL":
                        ConnString = "Server=" + this.HOST + ";Port=" + this.DBPORT + ";User Id=" + this.USER + ";Password=" + this.PASS + ";Database=" + this.DBNAME + ";";
                        AssemblyFile = Diretorio + "/Npgsql.dll";
                        AssemblyInstance = "Npgsql.NpgsqlConnection";//System.Data.SqlClient.SqlClientFactory
                        break;
                    default:
                        throw new Exception("Banco de dados " + this.DATABASE.ToUpper() + " não suportado.");
                }
                this.Conn = (IDbConnection)Assembly.LoadFrom(AssemblyFile).CreateInstance(AssemblyInstance);
                this.Conn.ConnectionString = ConnString;

                this.Conn.Open();

                if (!(this.Status()))
                {
                    throw new Exception(this.ErrorMsg);
                }

                return true;
            }
            catch (Exception e)
            {
                var error = e.Message + Environment.NewLine + e.StackTrace;
                this.ErrorMsg = error;
                return false;
            }
        }

        public bool OpenWithParameters(String _DATABASE,
                                        String _HOST,
                                        String _USER,
                                        String _PASS,
                                        String _DBNAME,
                                        String _DBPORT,
                                        String _LOCALE,
                                        String _PROTOCOL,
                                        String _SERVER,
                                        String _SID)
        {

            this.DATABASE = _DATABASE;
            this.HOST = _HOST;
            this.USER = _USER;
            this.PASS = _PASS;
            this.DBNAME = _DBNAME;
            this.DBPORT = _DBPORT;
            this.LOCALE = _LOCALE;
            this.PROTOCOL = _PROTOCOL;
            this.SERVER = _SERVER;
            this.SID = _SID;

            try
            {

                if (!Sistema.ReadConfig())
                {

                }

                try
                {
                    if (this.Status())
                    {
                        return true;
                    }
                }
                catch { }

                String ConnString = "";

                var Diretorio = this.ReadConfig();

                switch (this.DATABASE.ToUpper())
                {
                    case "MYSQL":
                        ConnString = "Server=" + this.HOST + ";Port=" + this.DBPORT + ";Database=" + this.DBNAME + ";Uid=" + this.USER + ";Pwd=" + this.PASS + ";Pooling=True;CharSet=utf8;Connection Timeout=300;Default Command Timeout=300;";
                        AssemblyFile = Diretorio + "MySql.Data.dll";
                        AssemblyInstance = "MySql.Data.MySqlClient.MySqlConnection";
                        break;
                    case "SQLSERVER":
                        ConnString = "Server=" + this.HOST + "," + this.DBPORT + ";Database=" + this.DBNAME + ";User Id=" + this.USER + ";Password=" + this.PASS;
                        AssemblyFile = Diretorio + "System.Data.dll";
                        AssemblyInstance = "System.Data.SqlClient.SqlConnection";
                        break;
                    case "ORACLE":
                        ConnString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + this.HOST + ")(PORT=" + this.DBPORT + "))(CONNECT_DATA=(SERVICE_NAME=" + this.SID + ")));User Id=" + this.USER + ";Password=" + this.PASS + ";Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;Incr Pool Size=5;Decr Pool Size=2";
                        AssemblyFile = Diretorio + "Oracle.ManagedDataAccess.dll";
                        AssemblyInstance = "Oracle.ManagedDataAccess.Client.OracleConnection";
                        break;
                    case "POSTGRESQL":
                        ConnString = "Server=" + this.HOST + ";Port=" + this.DBPORT + ";User Id=" + this.USER + ";Password=" + this.PASS + ";Database=" + this.DBNAME + ";";
                        AssemblyFile = Diretorio + "Assembly/Npgsql.dll";
                        AssemblyInstance = "Npgsql.NpgsqlConnection";//System.Data.SqlClient.SqlClientFactory
                        break;
                    default:
                        throw new Exception("Banco de dados " + this.DATABASE.ToUpper() + " não suportado.");
                }
                this.Conn = (IDbConnection)Assembly.LoadFrom(AssemblyFile).CreateInstance(AssemblyInstance);
                this.Conn.ConnectionString = ConnString;

                this.Conn.Open();

                if (!(this.Status()))
                {
                    throw new Exception(this.ErrorMsg);
                }

                return true;
            }
            catch (Exception e)
            {
                var error = e.Message + Environment.NewLine + e.StackTrace;
                this.ErrorMsg = error;
                return false;
            }
        }

        public void Begin()
        {
            try
            {
                this.Transacao = this.Conn.BeginTransaction();
            }
            catch (Exception e)
            {
                //Sistema.Log(e);
            }
        }
        public void Rollback()
        {
            try
            {
                this.Transacao.Rollback();
                this.Transacao.Dispose();
            }
            catch { }
        }
        public void Commit()
        {
            try
            {
                this.Transacao.Commit();
                this.Transacao.Dispose();
            }
            catch { }
        }
        public bool ReadConfig()
        {
            try
            {
                var Diretorio = Sistema.RootPath();
                String config = Diretorio + "/DBConfig.ini";
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
                            case "[CONNECTION SETTINGS]":
                                break;
                            case "DATABASE":
                                this.DATABASE = valor;
                                break;
                            case "HOST":
                                this.HOST = valor;
                                break;
                            case "USER":
                                this.USER = valor;
                                break;
                            case "PASS":
                                this.PASS = valor;//Sistema.DecodeBase64(valor);
                                break;
                            case "DBNAME":
                                this.DBNAME = valor;
                                break;
                            case "DBPORT":
                                this.DBPORT = valor;
                                break;
                            case "LOCALE":
                                this.LOCALE = valor;
                                break;
                            case "PROTOCOL":
                                this.PROTOCOL = valor;
                                break;
                            case "SERVER":
                                this.SERVER = valor;
                                break;
                            case "SID":
                                this.SID = valor;
                                break;
                            case "ERPNAME":
                                this.ERPNAME = valor;
                                break;
                            default:
                                throw new Exception("Parâmetro " + campo + " não foi reconhecido nas configurações de conexão com banco de dados.");
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                this.ErrorMsg = e.Message.ToString();
                return false;
            }
        }

        public bool Status()
        {
            try
            {
                if (!(this.Conn.State == ConnectionState.Open))
                {
                    throw new Exception("A conexão foi fechada de forma inesperada.");
                }
                return true;
            }
            catch (Exception e)
            {
                this.ErrorMsg = e.Message.ToString();
                return false;
            }

        }
        public bool Query(String query, bool UsingIsolation = false)
        {
            try
            {
                if (!(this.Status()))
                {
                    //this.Open();
                }



                CultureInfo ciAtual = Thread.CurrentThread.CurrentCulture;
                //Altero a cultura corrente para "en-US"
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                if (UsingIsolation && this.DATABASE == "INFORMIX")
                {
                    IDbCommand cmd2 = this.Conn.CreateCommand();
                    cmd2.CommandText = "SET ISOLATION TO DIRTY READ";
                    cmd2.ExecuteReader();
                }

                IDbCommand cmd = this.Conn.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = query;
                cmd.Transaction = this.Transacao;
                this.dr = cmd.ExecuteReader();
                this.dt = new DataTable(); //DATATABLE CONSIDERA AS PRIMARY KEY DO BANCO DE DADOS E IGNORA REGISTROS DUPLICADOS.
                                           //SEMPRE UTILIZAR ID OU TODAS AS PRIMARY KEY PARA QUE O DATATABLE NAO OMITA NENHUMA LINHA.
                this.dt.Load(this.dr);

                debug("SQL EXECUTADO COM SUCESSO: " + query + "\r\n");

                var dadoslog = "";

                if (Startup.Parametros["ativar_log_query"] == "S" && Startup.Parametros["ativar_log"] == "S")
                {
                    dadoslog += "NUMERO DE LINHAS RETORNADAS: " + this.getRows() + "\r\n";

                    var carregoudatacolumn = false;
                    var dadosColunas = new List<string>();

                    if (this.getRows() > 0)
                    {
                        dadoslog += "DADOS RETORNADOS:\r\n";

                        int i = 1;
                        foreach (DataRow row in this.dt.Rows)
                        {
                            dadoslog += "    LINHA: " + i + "\r\n";

                            if (carregoudatacolumn == false)
                            {
                                foreach (DataColumn col in row.Table.Columns)
                                {
                                    dadoslog += "          " + col.ColumnName.PadRight(20, ' ') + " -> " + row[col.ColumnName].ToString() + "\r\n";
                                    if (!(dadosColunas.Contains((col.ColumnName))))
                                    {
                                        dadosColunas.Add(col.ColumnName);
                                    }
                                }
                                carregoudatacolumn = true;
                            }
                            else
                            {
                                foreach (string col in dadosColunas)
                                {
                                    dadoslog += "          " + col.PadRight(20, ' ') + " -> " + row[col].ToString() + "\r\n";
                                }
                            }
                            dadoslog += "    FIM DA LINHA: " + i + "\r\n";

                            i++;
                        }
                    }
                    debug(dadoslog);
                }
                return true;
            }
            catch (Exception e)
            {
                this.ErrorMsg = e.Message + " SQL: " + query;
                debug("ERRO AO EXECUTAR QUERY: " + e.Message + " \r\nSQL COM ERRO: " + query);
                debug(e.StackTrace);
                return false;
            }

        }

        public DataTable LoadDropListView(String query)
        {
            this.Query(query);
            return this.dt;
        }
        public DataTable getDataTable(String query, bool UsingIsolation = false)
        {
            if (!(this.Query(query, UsingIsolation)))
            {
                this.dt = new DataTable();
                //Message.Error(this.ErrorMsg);
            }
            return this.dt;
        }
        public bool Execute(String sql)
        {
            try
            {
                if (!(this.Status()))
                {
                    //this.Open();
                }
                CultureInfo ciAtual = Thread.CurrentThread.CurrentCulture;
                //Altero a cultura corrente para "en-US"
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                debug("SQL ORIGINAL: " + sql);

                for (int i = 0; i < 99; i++)
                {
                    sql = sql.Replace("''", "NULL").Replace("'0001-01-01'", "NULL");
                }

                for (int i = 0; i < 99; i++)
                {
                    sql = sql.Replace("'LEAVEBLANK'", "' '");
                }

                IDbCommand cmd = this.Conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Transaction = this.Transacao;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                debug("SQL EXECUTADO COM SUCESSO: " + sql);
                return true;
            }
            catch (Exception e)
            {
                //this.Rollback();
                this.ErrorMsg = e.Message + e.InnerException + e.StackTrace + e.Data + " SQL: " + sql;
                debug("ERRO AO EXECUTAR QUERY: " + e.Message + " \r\nSQL COM ERRO: " + sql);
                return false;
            }
        }

        public void debug(string msg, string ArqNome = "", bool Forcar = false)
        {
            try
            {
                var debug_ativado = false;
                if (Startup.Parametros.ContainsKey("ativar_log") && Startup.Parametros.ContainsKey("ativar_log_query"))
                {
                    if ((Startup.Parametros["ativar_log"] == "S" && Startup.Parametros["ativar_log_query"] == "S") || Forcar == true)
                    {
                        debug_ativado = true;
                    }
                }
                else
                {
                    if (Forcar == true)
                    {
                        debug_ativado = true;
                    }
                }

                if (debug_ativado)
                {
                    var Diretorio = AppDomain.CurrentDomain.BaseDirectory.ToString().Replace("\\", "/");
                    var Log = "";
                    if (ArqNome == "")
                    {
                        Log = Diretorio + "/Log/" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    }
                    else
                    {
                        Log = Diretorio + "/Log/" + ArqNome;
                    }
                    msg = "\r\n[" + DateTime.Now.ToString("HH:mm:ss") + "] - " + msg;
                    File.AppendAllText(Log, msg);
                }
            }
            catch
            {

            }
        }

        public Dictionary<int, Dictionary<string, string>> getAllData(string query)
        {
            var dic = new Dictionary<int, Dictionary<string, string>>();

            int ind = 0;
            DataTable dattab = this.getDataTable(query);
            foreach (DataRow row in dattab.Rows) // Loop over the rows.
            {
                var dic2 = new Dictionary<string, string>();
                foreach (DataColumn column in dattab.Columns)
                {
                    if (dic2.ContainsKey(column.ToString()))
                    {
                        dic2[column.ToString()] = row[column].ToString();
                    }
                    else
                    {
                        dic2.Add(column.ToString(), row[column].ToString());
                    }
                }
                dic.Add(ind, dic2);
                ind++;
            }

            return dic;
        }
        public int getRows()
        {
            return this.dt.Rows.Count;
        }
        public int Count(string query)
        {
            this.Query(query);
            return this.dt.Rows.Count;
        }
        public String getValueByIndex(int index)
        {
            try
            {
                this.dr.Read();
                if (this.dr.IsDBNull(index))
                {
                    String retorno = this.dr.GetString(index);
                    this.dr.Close();
                    return retorno;
                }
                throw new Exception("Retorno está vazio");

            }
            catch (Exception e)
            {
                this.dr.Close();
                this.ErrorMsg = e.Message;
                return string.Empty;
            }
        }
        public String getValueByName(String name, String Default = "")
        {
            try
            {
                if (this.dt.Rows[0][name].ToString() == string.Empty)
                {
                    return Default;
                }
                else
                {
                    return this.dt.Rows[0][name].ToString();
                }
            }
            catch (Exception e)
            {
                this.ErrorMsg = e.Message;
                return Default;
            }
        }


        public void Close()
        {
            try
            {
                if (this.Conn != null)
                {
                    this.Conn.Close();
                    this.Conn.Dispose();
                }
            }
            catch (Exception e)
            {
                this.ErrorMsg = e.Message;
            }
        }


        public string Decimal(string val)
        {
            try
            {
                if (val.IndexOf("R$") != -1)
                {
                    val = val.Replace("R$", "").Trim();
                }

                var culture = new System.Globalization.CultureInfo("en-US");
                string ret = System.Decimal.Parse(val).ToString("N2", culture);
                return ret.Replace(",", "");
            }
            catch (Exception e)
            {
                return val;
            }
        }

        public string Date(string val)
        {
            try
            {
                return Convert.ToDateTime(val).ToString("yyyy-MM-dd");
            }
            catch (Exception e)
            {
                return val;
            }
        }

        public string getNextID(string tabela, string campoID)
        {
            try
            {
                int intID = 0;

                var query = "";
                
                query = "SELECT valor FROM handson_controle_id WHERE tabela = '" + tabela + "'";
                this.Query(query);

                if (this.getRows() == 0 || this.getValueByName("valor") == "")
                {
                    query = "SELECT MAX(" + campoID + ") as " + campoID + " FROM " + tabela;
                    this.Query(query);
                    var max = this.getValueByName(campoID);
                    if (max != string.Empty)
                    {
                        intID = Convert.ToInt32(max);
                    }
                    intID++;
                    query = "DELETE FROM handson_controle_id WHERE tabela = '" + tabela + "'";
                    if (!(this.Execute(query)))
                    {
                        throw new Exception("Falha ao deletar dados na tabela handson_controle_id: " + this.ErrorMsg);
                    }
                    query = "INSERT INTO handson_controle_id VALUES ('" + tabela + "','" + intID + "')";
                    if (!(this.Execute(query)))
                    {
                        throw new Exception("Falha ao inserir dados na tabela handson_controle_id: " + this.ErrorMsg);
                    }
                }
                else
                {
                    intID = Convert.ToInt32(this.getValueByName("valor"));
                    intID++;
                    query = "UPDATE handson_controle_id SET valor = '" + intID + "' WHERE tabela = '" + tabela + "'";
                    if (!(this.Execute(query)))
                    {
                        throw new Exception("Falha ao atualizar dados na tabela handson_controle_id: " + this.ErrorMsg);
                    }
                }

                return intID.ToString();
            }
            catch (Exception e)
            {
                this.ErrorMsg = e.Message + Environment.NewLine + e.StackTrace;
                return String.Empty;
            }
        }
        

        public string DBDate(string date)
        {
            var newdate = "";
            switch (this.DATABASE)
            {
                case "INFORMIX":
                    newdate = date; // date.Substring(8, 2) + "/" + date.Substring(5, 2) + "/" + date.Substring(0, 4);
                                    //ALTERAR VARIAVEL DE AMBIENTE DBDATE PARA y4md-
                    break;
                default:
                    newdate = date;
                    break;
            }
            return newdate;
        }
        

        public string sqlTrim(string str)
        {
            var trimmed = "";
            switch (this.DATABASE)
            {
                case "SQLSERVER":
                    trimmed = "rtrim(ltrim(" + str + "))";
                    break;
                default:
                    trimmed = "trim(" + str + ")";
                    break;
            }

            return trimmed;
        }

        public string sqlConcat(string str)
        {
            var concatenated = "";
            switch (this.DATABASE)
            {
                case "SQLSERVER":
                    concatenated = "+" + str;
                    break;
                default:
                    concatenated = "||" + str;
                    break;
            }

            return concatenated;
        }


        public List<T> getList<T>(string query)
        {
            //INSTANCIA FORM DINAMICAMENTE
            //var assembly = Assembly.GetExecutingAssembly();
            //var type = assembly.GetTypes().First(t => t == tp);


            //Type listType = typeof(List<>).MakeGenericType(new Type[] { tp });
            //var objlist = Activator.CreateInstance(listType);
            var objlist = new List<T>();
            var type = typeof(T);

            DataTable dt = this.getDataTable(query);

            foreach (DataRow row in dt.Rows)
            {
                object obj = Activator.CreateInstance(type);
                foreach (DataColumn col in dt.Columns)
                {
                    try
                    {
                        PropertyInfo prop = type.GetProperty(col.ColumnName);
                        prop.SetValue(obj, row[col.ColumnName].ToString(), null);
                    }
                    catch { }
                }
                objlist.GetType().GetMethod("Add").Invoke(objlist, new[] { obj });

            }




            return objlist;
        }

        public T getFirst<T>(string query)
        {
            var type = typeof(T);

            DataTable dt = this.getDataTable(query);
            object obj = Activator.CreateInstance(type);

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    try
                    {
                        PropertyInfo prop = type.GetProperty(col.ColumnName);
                        prop.SetValue(obj, row[col.ColumnName].ToString(), null);
                    }
                    catch { }
                }
                break;
            }

            return (T)obj;
        }

        public bool verifica_se_tabela_existe(string tabela)
        {
            bool status = false;
            var query = "";
            switch (this.DATABASE)
            {
                case "POSTGRESQL":
                    query = $"SELECT * FROM pg_catalog.pg_tables where tablename = '{tabela}'";
                    break;
                default:
                    break;
            }

            this.Query(query);
            if (this.getRows() > 0)
            {
                status = true;
            }
            return status;
        }

        public bool verifica_se_view_existe(string viewname)
        {
            bool status = false;
            var query = "";
            switch (this.DATABASE)
            {
                case "POSTGRESQL":
                    query = $"SELECT * FROM pg_catalog.pg_views where viewname = '{viewname}'";
                    break;
                default:
                    break;
            }

            this.Query(query);
            if (this.getRows() > 0)
            {
                status = true;
            }
            return status;
        }

        public string addZeroes(int num, int len)
        {
            var numberWithZeroes = num.ToString();
            var counter = numberWithZeroes.Length;
            while (counter < len)
            {
                numberWithZeroes = "0" + numberWithZeroes;
                counter++;
            }
            return numberWithZeroes;
        }

    }
}