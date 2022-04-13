using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using Newtonsoft.Json;
using System.Dynamic;

namespace API.Models
{
    public class Customizacao
    {
        public int cod_consulta { get; set; }
        public string apelido_consulta { get; set; }
        public string query { get; set; }
    }

    public class ConsultaCustomizada
    {
        private int _cod_consulta;
        private int _cod_usuario;
        private int _cod_empresa;
        private int _cod_estabelecimento;
        private string _apelido_consulta;
        private string _query;

        public int CodConsulta
        {
            get 
            { return _cod_consulta; }
            set
            {
                if (value == 0)
                    throw new Exception("Codigo Consulta Obrigatório, Favor informar");
                else
                {
                    _cod_consulta = value;
                }
            }
        }
        public int CodUsuario
        {
            get { return _cod_usuario; }
            set {
                if (value == 0)
                {
                    throw new Exception("Usuario Obrigatório, Favor informar");
                }
                else
                {
                    _cod_usuario = value;
                }
            }
        }

        public int CodEmpresa
        {
            get { return _cod_empresa; }
            set
            {
                if (value == 0)
                {
                    throw new Exception("Empresa Obrigatório, Favor informar");
                }
                else
                {
                    _cod_empresa = value;
                }
            }
        }

        public int CodEstabelecimento
        {
            get { return _cod_estabelecimento; }
            set
            {
                if (value == 0)
                {
                    throw new Exception("Estabelecimento Obrigatório, Favor informar");
                }
                else
                {
                    _cod_estabelecimento = value;
                }
            }
        }

        public string Apelido
        {
            get { return _apelido_consulta; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new Exception("Apelido Obrigatório, Favor informar");
                }
                else
                {
                    _apelido_consulta = value;
                }
            }
        }

        public string Query
        {
            get { return _query; }
            set { _query = (String.IsNullOrEmpty(value) ? throw new Exception("Não foi possivel seguir com a consulta Customizada. Query solicitada não encontrada") : value);}
        }

        private string getQueryCustomizacao()
        {
            var query = $"select * from consulta_customizada where 1 = 1";

            if (this.CodUsuario > 0)
            {
                query += $" and cod_usuario = {this.CodUsuario}";
            }

            if (this.CodEmpresa > 0)
            {
                query += $" and cod_empresa = {this.CodEmpresa}";
            }

            if (this.CodEstabelecimento > 0)
            {
                query += $" and cod_estabelecimento = {this.CodEstabelecimento}";
            }

            if (!String.IsNullOrEmpty(this.Apelido))
            {
                query += $" and apelido_consulta = '{this.Apelido}'";
            }

            if (this.CodConsulta > 0)
            {
                query += $" and cod_consulta = {this.CodConsulta}";
            }

            return query;
        }

        public bool validaCustomizacaoExistente()
        {
            DB connValida = new DB();
            bool status = false;
            if (connValida.Open())
            {
                var query = this.getQueryCustomizacao();
                connValida.Query(query);
                if (connValida.getRows() > 0)
                {
                    status = true;
                }
            }
            connValida.Close();
            return status;
        }

        public List<Dictionary<string, string>> returnConsulta()
        {   
            DB connConsulta = new DB();
            getConsultaCustomizadaRetorno retorno = new getConsultaCustomizadaRetorno();

            var list = new List<Dictionary<string, string>>();
            var obj = new Dictionary<string, string>();
            if (connConsulta.Open())
            {
                var query = this.getQueryCustomizacao();
                connConsulta.Query(query);
                this.Query = connConsulta.getValueByName("consulta");

                DataTable dtConsulta = connConsulta.getDataTable(this.Query);
                foreach (DataRow row in dtConsulta.Rows)
                {
                    obj = new Dictionary<string, string>();
                    foreach (DataColumn column in dtConsulta.Columns)
                    {
                        var tipo = column.DataType.ToString();
                        var valor_coluna = column.ColumnName.ToString();
                        var valor_linha = row[valor_coluna].ToString();
                        obj.Add(valor_coluna, valor_linha);    
                    }
                    list.Add(obj);
                }
            }
            else
            {
                throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
            }

            connConsulta.Close();
            return list;
        }
    }

    public class AllConsultas
    {
        private int _cod_usuario;
        private int _cod_empresa;
        private int _cod_estabelecimento;
        private string _query;

        public int CodUsuario
        {
            get { return _cod_usuario; }
            set
            {
                if (value == 0)
                {
                    throw new Exception("Usuario Obrigatório, Favor informar");
                }
                else
                {
                    _cod_usuario = value;
                }
            }
        }

        public int CodEmpresa
        {
            get { return _cod_empresa; }
            set
            {
                if (value == 0)
                {
                    throw new Exception("Empresa Obrigatório, Favor informar");
                }
                else
                {
                    _cod_empresa = value;
                }
            }
        }

        public int CodEstabelecimento
        {
            get { return _cod_estabelecimento; }
            set
            {
                if (value == 0)
                {
                    throw new Exception("Estabelecimento Obrigatório, Favor informar");
                }
                else
                {
                    _cod_estabelecimento = value;
                }
            }
        }

        private string getQueryCustomizacao()
        {
            var query = $"select * from consulta_customizada where 1 = 1";

            if (this.CodUsuario > 0)
            {
                query += $" and cod_usuario = {this.CodUsuario}";
            }

            if (this.CodEmpresa > 0)
            {
                query += $" and cod_empresa = {this.CodEmpresa}";
            }

            if (this.CodEstabelecimento > 0)
            {
                query += $" and cod_estabelecimento = {this.CodEstabelecimento}";
            }

            return query;
        }

        public bool validaCustomizacaoExistente()
        {
            DB conn = new DB();
            bool status = false;

            var query = this.getQueryCustomizacao();
            conn.Query(query);
            if (conn.getRows() > 0)
            {
                status = true;
            }
            conn.Close();
            return status;
        }

        public List<Dictionary<string, string>> returnConsulta()
        {
            DB connConsulta = new DB();
            getConsultaCustomizadaRetorno retorno = new getConsultaCustomizadaRetorno();

            var list = new List<Dictionary<string, string>>();
            var obj = new Dictionary<string, string>();
            if (connConsulta.Open())
            {
                var query = this.getQueryCustomizacao();
                connConsulta.Query(query);

                DataTable dtConsulta = connConsulta.getDataTable(query);
                foreach (DataRow row in dtConsulta.Rows)
                {
                    obj = new Dictionary<string, string>();
                    foreach (DataColumn column in dtConsulta.Columns)
                    {
                        var tipo = column.DataType.ToString();
                        var valor_coluna = column.ColumnName.ToString();
                        var valor_linha = row[valor_coluna].ToString();
                        obj.Add(valor_coluna, valor_linha);
                    }
                    list.Add(obj);
                }
            }
            else
            {
                throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
            }

            connConsulta.Close();
            return list;
        }
    }
}