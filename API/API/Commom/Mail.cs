using System;
using API.Models;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Pop3;
using MailKit;
using MimeKit;
using System.IO;
using System.Linq;
using MailKit.Net.Imap;
using System.Collections.Generic;

namespace API
{
    class Mail
    {
        /*
        public static string ErrorMsg = "";

        public static void Notificacoes()
        {
            DB Conn = new DB();
            try
            {
                
                if (Conn.Open())
                {
                    var StatusToSend = "'P'";
                    if (WebApiApplication.Parametros["reenviar_notif_erro"] == "S")
                    {
                        StatusToSend += ",'E'";
                    }

                    var entrou = false;

                    var query = "SELECT * FROM bus_notificacoes WHERE status IN (" + StatusToSend + ")";
                    DataTable dt = Conn.getDataTable(query);

                    foreach (DataRow row in dt.Rows)
                    {
                        entrou = true;
                        Sistema.LogJob("NOTIFICACAO: " + row["id"].ToString());
                        AtualizaStatus(row["id"].ToString(), "Z", "Notificação está sendo enviada", Conn);
                        if (row["destinatario"].ToString() == string.Empty)
                        {
                            AtualizaStatus(row["id"].ToString(), "E", "Nenhum destinatário informado", Conn);
                            Sistema.LogJob("NENHUM DESTINATARIO INFORMADO");
                            continue;
                        }

                        Sistema.LogJob("ENVIANDO...");
                        SendAsync(row["id"].ToString(), row["destinatario"].ToString(), row["assunto"].ToString(), row["mensagem"].ToString());
                    }
                    if (!entrou)
                    {
                        Sistema.LogJob("NAO HA NOTIFICACOES PARA ENVIAR");
                    }
                }
                else
                {
                    throw new Exception(Conn.ErrorMsg);
                }
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message + Environment.NewLine + e.StackTrace;
            }
            finally
            {
                Conn.Close();
            }
        }

        public static void EnvioAutomatico(DB Conn = null)
        {
            Sistema.LogJob("====================== INICIO NOTIFICACOES ==========================");

            if (WebApiApplication.Parametros["ativar_notificacoes"] == "N")
            {
                Sistema.LogJob("PARAMETRO ativar_notificacoes ESTA DESABILITADO");
                Sistema.LogJob("NENHUMA NOTIFICACAO SERA ENVIADA");
            }
            else
            {

                var ConnInterna = false;
                if (Conn == null)
                {
                    ConnInterna = true;
                    Conn = new DB();
                    Conn.Open();
                }
                try
                {
                    Sistema.LogJob("=============== VERIFICA TIMEOUT DAS NOTIFICACOES ==================");
                    NotifTimeOut(Conn);
                    Sistema.LogJob("================= VERIFICA NOTIFICACOES A ENVIAR ====================");
                    Notificacoes();

                }
                catch (Exception e)
                {
                    ErrorMsg = e.Message + Environment.NewLine + e.StackTrace;
                }
                finally
                {
                    if (ConnInterna)
                    {
                        Conn.Close();
                    }
                }
            }
            Sistema.LogJob("======================== FIM NOTIFICACOES ==========================");
        }

        public static void NotifTimeOut(DB Conn=null)
        {
            var ConnInterna = false;
            if (Conn == null)
            {
                ConnInterna = true;
                Conn = new DB();
                Conn.Open();
            }
            try
            {
                var query = "SELECT * FROM bus_notificacoes WHERE status = 'Z'";
                DataTable dt = Conn.getDataTable(query);

                var entrou = false;

                foreach (DataRow row in dt.Rows)
                {
                    Sistema.LogJob("NOTIFICACAO: " + row["id"].ToString());
                    DateTime data = Convert.ToDateTime(row["data"].ToString());
                    int hora = Convert.ToInt32(row["hora"].ToString().Substring(0, 2));
                    int minute = Convert.ToInt32(row["hora"].ToString().Substring(3, 2));
                    int second = Convert.ToInt32(row["hora"].ToString().Substring(6, 2));
                    TimeSpan horas = new TimeSpan(hora, minute, second);
                    data = data.Date + horas;

                    Sistema.LogJob("DATA/HORA DA ULTIMA INTERACAO: " + data);

                    var qtdminutostimeout = -15; //qtd minutos para timeout
                    DateTime dataagora = DateTime.Now.AddMinutes(qtdminutostimeout);

                    Sistema.LogJob("DATA/HORA PARA TIMEOUT: " + dataagora);

                    if (data <= dataagora)
                    {
                        Sistema.LogJob("NOTIFICACAO EXCEDEU O TEMPO LIMITE: TIMEOUT");
                        AtualizaStatus(row["id"].ToString(), "E", "Time out", Conn);
                        entrou = true;
                    }
                }

                if (!entrou)
                {
                    Sistema.LogJob("NAO HA NOTIFICACOES QUE EXCEDERAM O TEMPO LIMITE PARA ENVIO");
                }
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message + Environment.NewLine + e.StackTrace;
                Sistema.LogJob("PROBLEMA AO VERIFICAR NOTIFICACAO: " + ErrorMsg);
            }
            finally
            {
                if (ConnInterna)
                {
                    Conn.Close();
                }
            }
        }

        

        public async static void SendAsync(string id, string destinatarios, string assunto, string mensagem)
        {
            await Task.Run(async () =>
            {
                await Task.Run(() => Send(id, destinatarios, assunto, mensagem));
            });
        }

        public static void AtualizaStatus(string id, string status, string mensagem, DB Conn=null)
        {
            var ConnInterna = false;
            if (Conn == null)
            {
                ConnInterna = true;
                Conn = new DB();
                Conn.Open();
            }
            try
            {
                Sistema.LogJob("ATUALIZA STATUS: ID: "+id);
                Sistema.LogJob("ATUALIZA STATUS: STATUS: " + status);
                Sistema.LogJob("ATUALIZA STATUS: MENSAGEM: " + mensagem);
                var data = Conn.DBDate(DateTime.Now.ToString("yyyy-MM-dd"));
                var hora = DateTime.Now.ToString("HH:mm:ss");
                var query = "UPDATE bus_notificacoes " +
                            "   SET data = '" + data + "'" +
                            "      ,hora = '" + hora + "'" +
                            "      ,status = '" + status + "'" +
                            "      ,msg_retorno = '" + mensagem.Replace("'","") + "'" +
                            " WHERE id = '" + id + "'";
                if (!(Conn.Execute(query)))
                {
                    throw new Exception(Conn.ErrorMsg);
                }
            }
            catch (Exception e)
            {
                ErrorMsg = e.Message + Environment.NewLine + e.StackTrace;
                Sistema.LogJob("ATUALIZA STATUS: PROBLEMA: " + ErrorMsg);
            }
            finally
            {
                if (ConnInterna)
                {
                    Conn.Close();
                }
            }
        }


        public static string IncluiNotificacao(string destinatarios, string assunto, string mensagem, DB Conn = null)
        {
            var retorno = "";
            if (WebApiApplication.Parametros["ativar_notificacoes"] == "S")
            {
                var ConnInterna = false;
                if (Conn == null)
                {
                    ConnInterna = true;
                    Conn = new DB();
                    Conn.Open();
                }
                try
                {
                    if (destinatarios == "")
                    {
                        var query2 = "SELECT email FROM bus_usuarios WHERE is_notificado = 'S'";
                        DataTable dt = Conn.getDataTable(query2);
                        foreach (DataRow row in dt.Rows)
                        {
                            if (!(destinatarios.Contains(row["email"].ToString())))
                            {
                                if (destinatarios == "")
                                {
                                    destinatarios = row["email"].ToString();
                                }
                                else
                                {
                                    destinatarios += ";" + row["email"].ToString();
                                }
                            }
                        }
                    }

                    if (destinatarios == "")
                    {
                        throw new Exception("Não há destinatários para notificar");
                    }

                    var data = Conn.DBDate(DateTime.Now.ToString("yyyy-MM-dd"));
                    var hora = DateTime.Now.ToString("HH:mm:ss");
                    var id = Conn.getNextID("bus_notificacoes", "id");
                    var query = "INSERT INTO bus_notificacoes VALUES ('" + id + "','" + destinatarios + "','" + assunto + "','" + mensagem + "','" + data + "','" + hora + "','P','Notificação incluída')";
                    if (!(Conn.Execute(query)))
                    {
                        throw new Exception(Conn.ErrorMsg);
                    }
                    retorno = "Notificação encaminhada para: " + destinatarios;
                }
                catch (Exception e)
                {
                    ErrorMsg = e.Message + Environment.NewLine + e.StackTrace;
                    retorno = e.Message;
                }
                finally
                {
                    if (ConnInterna)
                    {
                        Conn.Close();
                    }
                }
            }
            return retorno;
        }

        public static SMTP SmtpParams()
        {
            SMTP smtp = new SMTP();
            DB Conn = new DB();
            try
            {
                Conn.Open();

                var query = "SELECT * FROM bus_smtps ";
                Conn.Query(query);

                smtp.id_smtp = Conn.getValueByName("id_smtp");
                smtp.servidor = Conn.getValueByName("servidor");
                smtp.porta = Conn.getValueByName("porta");
                smtp.remetente = Conn.getValueByName("remetente");
                smtp.usuario = Conn.getValueByName("usuario");
                smtp.senha = Conn.getValueByName("senha");
                smtp.criptografia = Conn.getValueByName("criptografia");
                smtp.autenticacao = Conn.getValueByName("autenticacao");
            }
            catch
            {

            }
            finally
            {
                Conn.Close();
            }
            
            return smtp;
        }

        public static SmtpClient SmtpConfig()
        {
            SmtpClient smtpClient = new SmtpClient();

            if (WebApiApplication.SMTPParams.autenticacao == "S")
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(WebApiApplication.SMTPParams.usuario, Sistema.DecodeBase64(WebApiApplication.SMTPParams.senha));
            }

            smtpClient.Host = WebApiApplication.SMTPParams.servidor;
            smtpClient.Port = Convert.ToInt32(WebApiApplication.SMTPParams.porta);

            switch (WebApiApplication.SMTPParams.criptografia)
            {
                case "TLS":
                    smtpClient.EnableSsl = true;
                    break;
                case "SSL":
                    smtpClient.EnableSsl = true;
                    break;
                default:
                    smtpClient.EnableSsl = false;
                    break;
            }

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Timeout = 30000;
            
            return smtpClient;
        }

        public static bool Send(string id, string destinatarios, string assunto, string mensagem, SmtpClient smtpClient=null)
        {
            try
            {
                Sistema.LogJob("ENVIANDO NOTIFICACAO "+id+" -  PARA: " + destinatarios);
                Sistema.LogJob("ENVIANDO NOTIFICACAO " + id + " -  ASSUNTO: " + assunto);

                if (smtpClient == null)
                {
                    smtpClient = SmtpConfig();
                }

                MailMessage Email = new MailMessage();

                var splitdests = destinatarios.Split(';');
                foreach(string dest in splitdests)
                {
                    if (dest != string.Empty)
                    {
                        Email.To.Add(dest);
                    }
                }

                if (WebApiApplication.SMTPParams.remetente != null && WebApiApplication.SMTPParams.remetente != string.Empty)
                {
                    Email.From = new MailAddress(WebApiApplication.SMTPParams.remetente);
                }

                Email.Subject = assunto;
                Email.IsBodyHtml = true;
                Email.Body = mensagem;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                if (Email.To.Count > 0)
                {
                    smtpClient.Send(Email);
                    AtualizaStatus(id, "S", "Notificação enviado com sucesso");
                    Sistema.LogJob("ENVIANDO NOTIFICACAO " + id + " -  OK");
                }
                else
                {
                    AtualizaStatus(id, "E", "Nenhum destinatário informado no envio");
                    Sistema.LogJob("ENVIANDO NOTIFICACAO " + id + " - NENHUM DESTINATARIO INFORMADO NO ENVIO");
                }

                return true;
            }
            catch (Exception e)
            {
                try
                {
                    ErrorMsg = e.Message + Environment.NewLine + e.InnerException.Message + Environment.NewLine + e.StackTrace;
                }
                catch
                {
                    ErrorMsg = e.Message + Environment.NewLine + e.StackTrace;
                }
                Sistema.LogJob("ENVIANDO NOTIFICACAO " + id + " - PROBLEMA: " + ErrorMsg);
                AtualizaStatus(id, "E", ErrorMsg);
                return false;
            }
        }

        public static bool getPopAttachments(string host, int port, bool ssl, string user, string pass, List<string> arquivos = null)
        {
            try
            {
                using (var client = new Pop3Client())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(host, port, ssl);
                    client.Authenticate(user, pass);

                    if (arquivos == null)
                    {
                        arquivos = new List<string>();
                    }

                    if (arquivos.Count == 0)
                    {
                        arquivos.Add("%");
                    }

                    for (int i = 0; i < client.Count; i++)
                    {
                        var message = client.GetMessage(i);
                        foreach (var attachment in message.Attachments.OfType<TextPart>())
                        {
                            var fileName = attachment.FileName;
                            for (int i2 = 0; i2 < arquivos.Count; i2++)
                            {
                                if (Sistema.Like(fileName, arquivos[i2]))
                                {
                                    var Arquivo = getPathToFile(fileName);

                                    using (var stream = File.Create(Arquivo))
                                    {
                                        //GRAVA ARQUIVO
                                        attachment.Content.DecodeTo(stream);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception exc)
            {
                ErrorMsg = exc.Message + Environment.NewLine + exc.StackTrace;
                return false;
            }
        }


        public static bool getImapAttachments(string host, int port, bool ssl, string user, string pass, List<string> arquivos = null)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(host, port, ssl);
                    client.Authenticate(user, pass);
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    IMailFolder DestFolder;
                    try
                    {
                        DestFolder = client.GetFolder("Processados Visio BUS");
                    }
                    catch
                    {
                        var toplevel = client.GetFolder(client.PersonalNamespaces[0]);
                        var mailkit = toplevel.Create("Processados Visio BUS", true);
                        DestFolder = client.GetFolder("Processados Visio BUS");
                    }

                    if (arquivos == null)
                    {
                        arquivos = new List<string>();
                    }

                    if (arquivos.Count == 0)
                    {
                        arquivos.Add("%");
                    }
                    
                    while(inbox.Count > 0)
                    {
                        var message = inbox.GetMessage(0);
                        foreach (var attachment in message.Attachments.OfType<TextPart>())
                        {
                            var fileName = attachment.FileName;

                            for (int i = 0; i < arquivos.Count; i++)
                            {
                                if (Sistema.Like(fileName, arquivos[i]))
                                {
                                    var Arquivo = getPathToFile(fileName);

                                    using (var stream = File.Create(Arquivo))
                                    {
                                        //GRAVA ARQUIVO
                                        attachment.Content.DecodeTo(stream);
                                        break;
                                    }
                                }
                            }
                        }
                        
                        inbox.AddFlags(0, MessageFlags.Seen, true);
                        inbox.MoveTo(0, DestFolder);
                        
                    }
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception exc)
            {
                ErrorMsg = exc.Message + Environment.NewLine + exc.StackTrace;
                return false;
            }
        }

        private static string getPathToFile(string fileName)
        {
            //VERIFICA SE DIRETORIO TEMP EXISTE
            var dir = Sistema.RootPath() + "/Temp/";
            if (!(Directory.Exists(dir)))
            {
                Directory.CreateDirectory(dir);
            }

            //VERIFICA SE DIRETORIO ANEXOS EXISTE
            dir += "Anexos/";
            if (!(Directory.Exists(dir)))
            {
                Directory.CreateDirectory(dir);
            }

            //VERIFICA SE JA EXISTE ARQUIVO COM MESMO NOME
            var Arquivo = dir + fileName;
            var split = fileName.Split('.');

            var FileNameSemExt = "";
            for (int a = 0; a < (split.Count() - 1); a++)
            {
                FileNameSemExt += split[a];
            }

            //SE ARQUIVO NAO TIVER EXTENSAO
            if (FileNameSemExt == string.Empty)
            {
                FileNameSemExt = fileName;
            }

            var Extensao = split.Last();

            var ind = 0;
            while (true)
            {
                if (!(File.Exists(Arquivo)))
                {
                    break;
                }

                //ARQUIVO JA EXISTE, ALTERANDO NOME COM NUMERACAO
                Arquivo = dir + FileNameSemExt + "_" + ind + "." + Extensao;
                ind++;
            }

            return Arquivo;
        }
        */
    }
}
