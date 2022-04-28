using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class Empresa
    {
        public string cod_empresa { get; set; }
        public string descricao { get; set; }
        public string pathcfg { get; set; }
        public string ativa { get; set; }
        public string endereco { get; set; }
        public string bairro { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
        public string fone { get; set; }
        public string ramal { get; set; }
        public string cgc_cpf { get; set; }
        public string pais { get; set; }
        public string i_estadual { get; set; }
        public string apelido { get; set; }
        public string pathintegra { get; set; }
        public string inicioatividade { get; set; }
        public string nlivroinventario { get; set; }
        public string nregistro { get; set; }
        public string regjuntacomercial { get; set; }
        public string nomefantasia { get; set; }
        public string fone2_l { get; set; }
        public string ramal2_l { get; set; }
        public string contato_l { get; set; }
        public string fax_l { get; set; }
        public string mapa_l { get; set; }
        public string i_munic { get; set; }
        public string i_produtor { get; set; }
        public string contribuiicms { get; set; }
        public string end_elet { get; set; }
        public string homepage { get; set; }
        public string end_cob { get; set; }
        public string bairro_c { get; set; }
        public string munic_c { get; set; }
        public string uf_cob { get; set; }
        public string cep_c { get; set; }
        public string fone1_c { get; set; }
        public string ramal1_c { get; set; }
        public string fone2_c { get; set; }
        public string ramal2_c { get; set; }
        public string contato_c { get; set; }
        public string fax_c { get; set; }
        public string mapa_c { get; set; }
        public string end_ent { get; set; }
        public string bairro_e { get; set; }
        public string munic_e { get; set; }
        public string uf_e { get; set; }
        public string cep_e { get; set; }
        public string fone1_e { get; set; }
        public string ramal1_e { get; set; }
        public string fone2_e { get; set; }
        public string ramal2_e { get; set; }
        public string contato_e { get; set; }
        public string fax_e { get; set; }
        public string mapa_e { get; set; }
        public string cadastro { get; set; }
        public string pessoa { get; set; }
        public string suframa { get; set; }
        public string iestsubtrib { get; set; }
        public string planocontabil { get; set; }
        public string cobranca_empresa { get; set; }
        public string sintegra_conv { get; set; }
        public string sintegra_nat { get; set; }
        public string sintegra_finalid { get; set; }
        public string numero_l { get; set; }
        public string complemento_l { get; set; }
        public string sintegra_contato { get; set; }
        public string sintegra_ultdata { get; set; }
        public string contribuiipi { get; set; }
        public string substitutoicms { get; set; }
        public string substituidoicms { get; set; }
        public string emiteecf { get; set; }
        public string usaworkflow { get; set; }
        public string optantesimples { get; set; }
        public string optantesimplesfederal { get; set; }
        public string regimportador { get; set; }
        public string regsiscomex { get; set; }
        public string nomecontador { get; set; }
        public string crc { get; set; }
        public string cpfcontador { get; set; }
        public string assinatura { get; set; }
        public string spedc_codrespadmcadent { get; set; }
        public string spedc_codqualiass { get; set; }
        public string dataarquivamento { get; set; }
        public string dataarquivamentoconversao { get; set; }
        public string tipoempresa { get; set; }
        public string spedf_tipoatividade { get; set; }
        public string emailcontador { get; set; }
        public string dddcontador { get; set; }
        public string fonecontador { get; set; }
        public string regimetributarionfe { get; set; }
        public string segmer { get; set; }
        public string tipo_apuracao_lucro { get; set; }
        public string spedf_indicadoremitentenf { get; set; }
        public string usuariocep { get; set; }
        public string senhacep { get; set; }
        public string banco { get; set; }
        public string agencia { get; set; }
        public string conta { get; set; }
        public string cnpj_escritorio { get; set; }
        public string end_contador { get; set; }
        public string endnum_contador { get; set; }
        public string endcompl_contador { get; set; }
        public string bairro_contador { get; set; }
        public string cep_contador { get; set; }
        public string municip_contador { get; set; }
        public string incentivadorcultural { get; set; }
        public string usuario { get; set; }
        public string horacada { get; set; }
        public string locrecpad { get; set; }
        public string numero_e { get; set; }
        public string numero_c { get; set; }
        public string dtopcaoregtribut { get; set; }
        public string controle { get; set; }
        public string controle1 { get; set; }
        public string controle2 { get; set; }
        public string controle3 { get; set; }
        public string controle4 { get; set; }
        public string controle5 { get; set; }
        public string iphardlock { get; set; }
        public string portahardlock { get; set; }
        public string controle6 { get; set; }
        public string controle7 { get; set; }
        public string controle8 { get; set; }
        public string data { get; set; }
        public string controle10 { get; set; }
        public string controle9 { get; set; }
        public float ssllib { get; set; }
        public string caminhoarquivopfx { get; set; }
        public string numseriecertificado { get; set; }
        public string senhacertificado { get; set; }
        public int formaprecisao { get; set; }
    }


    public class PrecessoQuery : Empresa
    {
        private string getWhereClause(Empresa emp)
        {
            var whereClause = "";

            Type typeEmp = emp.GetType();
            foreach (PropertyInfo item in typeEmp.GetProperties())
            {
                string nome = item.Name;
                object valor = item.GetValue(emp, null);

                whereClause += (!String.IsNullOrEmpty(valor.ToString())) ? $" and {nome} = '{valor}'" : "";
            }

            return whereClause;
        }

        private (string,string) getValuesInsert(Empresa emp)
        {
            var into = "";
            var values = "";
            
            Type typeEmp = emp.GetType();
            foreach (PropertyInfo item in typeEmp.GetProperties())
            {
                string nome = item.Name;
                object valor = item.GetValue(emp, null);

                if (valor == null)
                    continue;

                if (!String.IsNullOrEmpty(valor.ToString()))
                {
                    var newvalor = (item.PropertyType == typeof(string)) ? valor = $"'{valor}'" : valor;

                    into += (into.Length > 0) ? $", {nome}" : nome;
                    values += (values.Length > 0) ? $", {newvalor}" : newvalor;
                }
            }

            into = $"({into})";
            values = $"({values})";
            return (into, values);
        }

        private string getValuesUpdate(Empresa emp)
        {
            var update = "";

            Type typeEmp = emp.GetType();
            foreach (PropertyInfo item in typeEmp.GetProperties())
            {
                string nome = item.Name;
                object valor = item.GetValue(emp, null);

                if (valor == null)
                    continue;

                if (!String.IsNullOrEmpty(valor.ToString()))
                {
                    var newvalor = (item.PropertyType == typeof(string)) ? valor = $"'{valor}'" : valor;
                    update += (update.Length > 0) ? $", {nome} = {newvalor}" : $" set {nome} = {newvalor}";
                }
                else
                {
                    continue;
                }
            }

            return update;
        }

        public List<Dictionary<string,string>> select(Empresa emp)
        {
            dynamic lista = new List<Dictionary<string, string>>();
            DB connSelect = new DB();

            try
            {
                if (connSelect.Open())
                {
                    var query = $"SELECT cod_empresa," +
                                $"       descricao," +
                                $"       pathcfg," +
                                $"       ativa," +
                                $"       endereco," +
                                $"       bairro," +
                                $"       municipio," +
                                $"       uf," +
                                $"       cep," +
                                $"       fone," +
                                $"       ramal," +
                                $"       cgc_cpf," +
                                $"       pais," +
                                $"       i_estadual," +
                                $"       apelido," +
                                $"       pathintegra," +
                                $"       inicioatividade," +
                                $"       nlivroinventario," +
                                $"       nregistro," +
                                $"       regjuntacomercial," +
                                $"       nomefantasia," +
                                $"       fone2_l," +
                                $"       ramal2_l," +
                                $"       contato_l," +
                                $"       fax_l," +
                                $"       mapa_l," +
                                $"       i_munic," +
                                $"       i_produtor," +
                                $"       contribuiicms," +
                                $"       end_elet," +
                                $"       homepage," +
                                $"       end_cob," +
                                $"       bairro_c," +
                                $"       munic_c," +
                                $"       uf_cob," +
                                $"       cep_c," +
                                $"       fone1_c," +
                                $"       ramal1_c," +
                                $"       fone2_c," +
                                $"       ramal2_c," +
                                $"       contato_c," +
                                $"       fax_c," +
                                $"       mapa_c," +
                                $"       end_ent," +
                                $"       bairro_e," +
                                $"       munic_e," +
                                $"       uf_e," +
                                $"       cep_e," +
                                $"       fone1_e," +
                                $"       ramal1_e," +
                                $"       fone2_e," +
                                $"       ramal2_e," +
                                $"       contato_e," +
                                $"       fax_e," +
                                $"       mapa_e," +
                                $"       cadastro," +
                                $"       pessoa," +
                                $"       suframa," +
                                $"       iestsubtrib," +
                                $"       planocontabil," +
                                $"       cobranca_empresa," +
                                $"       sintegra_conv," +
                                $"       sintegra_nat," +
                                $"       sintegra_finalid," +
                                $"       numero_l," +
                                $"       complemento_l," +
                                $"       sintegra_contato," +
                                $"       sintegra_ultdata," +
                                $"       contribuiipi," +
                                $"       substitutoicms," +
                                $"       substituidoicms," +
                                $"       emiteecf," +
                                $"       usaworkflow," +
                                $"       optantesimples," +
                                $"       optantesimplesfederal," +
                                $"       regimportador," +
                                $"       regsiscomex," +
                                $"       nomecontador," +
                                $"       crc," +
                                $"       cpfcontador," +
                                $"       assinatura," +
                                $"       spedc_codrespadmcadent," +
                                $"       spedc_codqualiass," +
                                $"       dataarquivamento," +
                                $"       dataarquivamentoconversao," +
                                $"       tipoempresa," +
                                $"       spedf_tipoatividade," +
                                $"       emailcontador," +
                                $"       dddcontador," +
                                $"       fonecontador," +
                                $"       regimetributarionfe," +
                                $"       segmer," +
                                $"       tipo_apuracao_lucro," +
                                $"       spedf_indicadoremitentenf," +
                                $"       usuariocep," +
                                $"       senhacep," +
                                $"       banco," +
                                $"       agencia," +
                                $"       conta," +
                                $"       cnpj_escritorio," +
                                $"       end_contador," +
                                $"       endnum_contador," +
                                $"       endcompl_contador," +
                                $"       bairro_contador," +
                                $"       cep_contador," +
                                $"       municip_contador," +
                                $"       incentivadorcultural," +
                                $"       usuario," +
                                $"       horacada," +
                                $"       locrecpad," +
                                $"       numero_e," +
                                $"       numero_c," +
                                $"       dtopcaoregtribut," +
                                $"       controle," +
                                $"       controle1," +
                                $"       controle2," +
                                $"       controle3," +
                                $"       controle4," +
                                $"       controle5," +
                                $"       iphardlock," +
                                $"       portahardlock," +
                                $"       controle6," +
                                $"       controle7," +
                                $"       controle8," +
                                $"       data," +
                                $"       controle10," +
                                $"       controle9," +
                                $"       ssllib," +
                                $"       caminhoarquivopfx," +
                                $"       numseriecertificado," +
                                $"       senhacertificado," +
                                $"       formaprecisao" +
                                $"FROM   acx_empresa WHERE 1 = 1 {getWhereClause(emp)}";

                    DataTable dt = connSelect.getDataTable(query);
                    var obj = new Dictionary<string, string>();
                    foreach (DataRow row in dt.Rows)
                    {
                        obj = new Dictionary<string, string>();
                        foreach (DataColumn column in dt.Columns)
                        {
                            var tipo = column.DataType.ToString();
                            
                            var valor_coluna = column.ColumnName.ToString();
                            var valor_linha = row[valor_coluna].ToString();
                            obj.Add(valor_coluna, valor_linha);
                        }
                        lista.Add(obj);
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            connSelect.Close(); 
            return lista;
        }

        public void insert(Empresa emp)
        {
            DB connInsert = new DB();
            bool status = true;
            try
            {
                if (connInsert.Open())
                {   
                    var valores = this.getValuesInsert(emp);

                    connInsert.Begin();
                    var query = $"INSERT INTO acx_empresa {valores.Item1} VALUES {valores.Item2}";
                    if (!connInsert.Execute(query))
                    {
                        throw new Exception($"Não foi possivel prosseguir com o cadastro de Empresa. ERRO -> {connInsert.ErrorMsg}");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                status = false;
                connInsert.Rollback();
                throw new Exception(e.Message);
            }

            connInsert.Commit();
            connInsert.Close(); 
        }

        public void update(Empresa emp)
        {
            DB connUpdate = new DB();
            bool status = true;

            try
            {
                if (connUpdate.Open())
                {
                    var update = this.getValuesUpdate(emp);

                    connUpdate.Begin();

                    if (String.IsNullOrEmpty(emp.cod_empresa))
                    {
                        throw new Exception("Código da Empresa não Informado favor validar.");
                    }

                    var query = $"UPDATE acx_empresa {update} WHERE cod_empresa = {emp.cod_empresa}"; 
                    if (connUpdate.Execute(query))
                    {
                        throw new Exception($"Não foi possivel prosseguir com o cadastro de Empresa. ERRO -> {connUpdate.ErrorMsg}");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
            }
            catch (Exception e)
            {
                status = false;
                connUpdate.Rollback();
                throw new Exception(e.Message);
            }

            connUpdate.Commit();
            connUpdate.Close();
        }
    }
}
