using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class Login
    {
        public int cod_usuario { get; set; }
        public string login { get; set; }
        public string nom_usuario  { get; set; }
        public string senha { get; set; }
        public string empresa { get; set; }
        public string nom_empresa { get; set; }
        public int estabelecimento { get; set; }
        public string nom_estabelecimento { get; set; }
    }

    public class Acesso
    {
        public int cod_usuario { get; set; }
        public string cod_empresa { get; set; }    
        public int cod_estabelecimento { get; set; }
        public string token { get; set; }
    }

    public class Empresa
    {
        public string cod_empresa { get; set; }
        public string nom_empresa { get; set; }
        public List<Estabelecimento> listaEstabelecimento { get; set; }
    }

    public class Estabelecimento
    {
        public int cod_estabelecimento { get; set; }
        public string nom_estabelecimento { get; set; }
    }

    public class Vinculo
    {
        public int cod_usuario { get; set; }
        public string nom_usuario { get; set; }
        public List<Empresa> listaEmpresa { get; set; }
    }

    public class Loginacx
    {
        private int _cod_acesso;
        private int _cod_usuario;
        private string _cod_empresa;
        private int _cod_estabelecimento;
        private string _token;
        private string _usuario;
        private string _senha;
        public int CodAcesso
        {
            get { return _cod_acesso; }
            set { _cod_acesso = value; }
        }

        public int CodUsuario
        {
            get { return _cod_usuario; }
            set { _cod_usuario = value; }
        }

        public string CodEmpresa
        {
            get { return _cod_empresa; }
            set { _cod_empresa = value; }
        }

        public int CodEstabelecimento
        {
            get { return _cod_estabelecimento; }
            set { _cod_estabelecimento = value; }
        }
        public string Token
        {
            get { return _token; }
            set { _token = (String.IsNullOrEmpty(value)) ? throw new Exception("Token Inválido. Favor efetuar o login novamente.") : value; }
        }

        public string Usuario
        {
            get { return _usuario; }
            set { _usuario = (String.IsNullOrEmpty(value)) ? throw new Exception("Usuário Inválido. Favor verificar") : value; }
        }

        public string Senha
        {
            get { return _senha; }
            set { _senha = (String.IsNullOrEmpty(value)) ? throw new Exception("Senha Inválida. Favor verificar") : value; }
        }

        public string getQueryControleAcesso()
        {
            var query = $"select * from acx_controle_acessos where 1 = 1";

            if (this.CodUsuario > 0)
            {
                query += $" and cod_usuario = {this.CodUsuario}";
            }

            if (!String.IsNullOrEmpty(this.CodEmpresa))
            {
                query += $" and cod_empresa = '{this.CodEmpresa}'";
            }

            if (this.CodEstabelecimento > 0)
            {
                query += $" and cod_estabelecimento = {this.CodEstabelecimento}";
            }

            if (!String.IsNullOrEmpty(this.Token))
            {
                query += $" and chave_acesso = '{this.Token}'";
            }

            return query;
        }

        public string getQueryDadosUsuario()
        {
            string query = $"SELECT  d.cod_usuario," +
                           $"        d.nom_usuario," +
                           $"        a.*," +
                           $"        b.cod_estabelecimento," +
                           $"        b.nom_estabelecimento" +
                           $" FROM acx_empresa a," +
                           $"      acx_estabelecimento b," +
                           $"      acx_vinculo_usuario_emp c," +
                           $"      acx_usuarios d" +
                           $" WHERE a.cod_empresa = b.cod_empresa" +
                           $"   AND a.cod_empresa = c.cod_empresa" +
                           $"   AND d.cod_usuario = c.cod_usuario" +
                           $"   AND d.login = '{this.Usuario}'" +
                           $"   AND d.senha = '{this.Senha}'" +
                           $"   AND d.status_ativo = 'S'" +
                           $"   and b.cod_estabelecimento = {this.CodEstabelecimento}" +
                           $"   and a.cod_empresa = '{this.CodEmpresa}'";

            return query;
        }

        public string getQueryRemoveSessaoAtiva()
        {
            string query = $"delete from acx_controle_acessos where 1 = 1 ";
            if (this.CodUsuario > 0)
            {
                query += $" and cod_usuario = {this.CodUsuario}";
            }

            if (!String.IsNullOrEmpty(this.CodEmpresa))
            {
                query += $" and cod_empresa = '{this.CodEmpresa}'";
            }

            if (this.CodEstabelecimento > 0)
            {
                query += $" and cod_estabelecimento = {this.CodEstabelecimento}";
            }

            if (!String.IsNullOrEmpty(this.Token))
            {
                query += $" and cod_estabelecimento = {this.Token}";
            }

            return query;
        }

        public class getVinculo : Loginacx
        {
            private Vinculo _vinculo = new Vinculo();
            private string _login;
            private string _senha;
            private string _status_ativo;
            private bool _valida_usuario_ativo;

            public string Login
            {
                get { return _login; }

                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new Exception("Login Obrigatório, Favor informar");
                    }
                    else
                    {
                        _login = value;
                    }
                }
            }

            public string Senha
            {
                get { return _senha; }

                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new Exception("Senha Obrigatório, Favor informar");
                    }
                    else
                    {
                        _senha = value;
                    }
                }
            }

            public string StatusAtivo
            {
                get { return _status_ativo; }
                set { _status_ativo = value; }
            }

            public getVinculo(bool valida_usuario_ativo = false)
            {
                this._valida_usuario_ativo = valida_usuario_ativo;
            }

            public Object getList()
            {
                DB connVinculo = new DB();
                List<Vinculo> list = new List<Vinculo>();
                Vinculo vinculo = new Vinculo();
                vinculo.listaEmpresa = new List<Empresa>();

                string query;

                if (connVinculo.Open())
                {
                    if (this._valida_usuario_ativo)
                    {
                        if (string.IsNullOrEmpty(this.Login) || string.IsNullOrEmpty(this.Senha))
                        {
                            throw new Exception("Login ou Senha inválido.");
                        }

                        query = $"select cod_login from acx_usuarios where login = '{this.Login}' and senha = '{this.Senha}' and status_ativo = 'S'";
                        connVinculo.Query(query);
                        if (connVinculo.getRows() == 0)
                        {
                            throw new Exception("Usuário inativo. Não foi possivel prosseguir com a solicitação");
                        }
                    }

                    query = $"select cod_usuario, nom_usuario " +
                            $" from acx_usuarios " +
                            $" where login = '{this.Login}' " +
                            $" and senha = '{this.Senha}' " +
                            $" and status_ativo = '{this.StatusAtivo}'";
                    connVinculo.Query(query);


                    if (connVinculo.getRows() > 0)
                    {
                        vinculo.cod_usuario = int.Parse(connVinculo.getValueByName("cod_usuario"));
                        vinculo.nom_usuario = connVinculo.getValueByName("nom_usuario");

                        query = $"select a.*, b.descricao " +
                                $" from acx_vinculo_usuario_emp a, " +
                                $" acx_empresa b " +
                                $" where a.cod_empresa = b.cod_empresa " +
                                $" and a.cod_usuario = {vinculo.cod_usuario}";
                        DataTable dadosEmp = connVinculo.getDataTable(query);

                        int linhas = dadosEmp.Rows.Count;
                        if (linhas > 0)
                        {
                            foreach (DataRow row in dadosEmp.Rows)
                            {
                                Empresa emp = new Empresa();
                                emp.listaEstabelecimento = new List<Estabelecimento>();

                                emp.cod_empresa = row["cod_empresa"].ToString();
                                emp.nom_empresa = row["descricao"].ToString();

                                query = $"select cod_estabelecimento, nom_estabelecimento from acx_estabelecimento where cod_empresa = '{emp.cod_empresa}'";
                                DataTable dadosEstab = connVinculo.getDataTable(query);

                                linhas = dadosEstab.Rows.Count;
                                if (linhas > 0)
                                {
                                    foreach (DataRow row1 in dadosEstab.Rows)
                                    {
                                        Estabelecimento est = new Estabelecimento();

                                        est.cod_estabelecimento = int.Parse(row1["cod_estabelecimento"].ToString());
                                        est.nom_estabelecimento = row1["nom_estabelecimento"].ToString();

                                        emp.listaEstabelecimento.Add(est);
                                    }
                                }
                                else
                                {
                                    throw new Exception($"Usuário {this.Login} não possui vinculo com Empresa / Estabelecimento.");
                                }

                                vinculo.listaEmpresa.Add(emp);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                connVinculo.Close();
                return vinculo;
            }
        }

        public class DadosUsuario : Loginacx
        {
            public DadosUsuario(string chave)
            {
                this.Token = chave;
                this.getDados();
            }

            public void getDados()
            {
                DB connAcesso = new DB();
                if (connAcesso.Open())
                {
                    var query = this.getQueryControleAcesso();
                    connAcesso.Query(query);

                    if (connAcesso.getRows() > 0)
                    {
                        this.CodAcesso = int.Parse(connAcesso.getValueByName("cod_acesso"));
                        this.CodUsuario = int.Parse(connAcesso.getValueByName("cod_usuario"));
                        this.CodEmpresa = connAcesso.getValueByName("cod_empresa");
                        this.CodEstabelecimento = int.Parse(connAcesso.getValueByName("cod_estabelecimento"));
                        this.Token = connAcesso.getValueByName("chave_acesso");
                    }
                    else
                    {
                        throw new Exception("Não a dados a serem capturados referente a chave de acesso.");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                connAcesso.Close();
            }
        }

        public class Sessao : Loginacx
        {
            public bool validaSessaoAtiva()
            {
                DB connAcesso = new DB();
                bool status = false;
                if (connAcesso.Open())
                {
                    var query = this.getQueryControleAcesso();
                    connAcesso.Query(query);

                    if (connAcesso.getRows() > 0)
                    {
                        status = true;
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                connAcesso.Close();
                return status;
            }
        }
    }
}