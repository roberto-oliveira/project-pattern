using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Common
{
    public abstract class ServiceCommon
    {
        private string PopulateBody(string userName, string title, string url, string description)
        {
            string body = string.Empty;

            using (StreamReader reader = new StreamReader(Path.GetFileName("~/EmailTemplate.htm")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", userName);
            body = body.Replace("{Title}", title);
            body = body.Replace("{Url}", url);
            body = body.Replace("{Description}", description);
            return body;
        }

        protected void SendEmail(object sender, EventArgs e)
        {
            string body = this.PopulateBody("John",
                "Fetch multiple values as Key Value pair in ASP.Net AJAX AutoCompleteExtender",
                "//www.aspsnippets.com/Articles/Fetch-multiple-values-as-Key-Value-pair-" +
                "in-ASP.Net-AJAX-AutoCompleteExtender.aspx",
                "Here Mudassar Ahmed Khan has explained how to fetch multiple column values i.e." +
                " ID and Text values in the ASP.Net AJAX Control Toolkit AutocompleteExtender"
                + "and also how to fetch the select text and value server side on postback");
            this.EnviarEmail("recipient@gmail.com", "New article published!", body);
        }

        public void EnviarEmailComTemplate()
        {
            WebClient wc = new WebClient();
            wc.Encoding = System.Text.Encoding.UTF8;

            //Obtendo o conteúdo do template
            string sTemplate = wc.DownloadString(
                "http://www.cbsa.com.br/exemplos/template.html");

            //Mensagem para inserir no template
            string sMensagem = "Uma forma simples de enviar email com template HTML.";

            //fazendo o replace de ##Mensagem## por sMensagemno conteúdo obtido
            sTemplate = sTemplate.Replace("##Mensagem##", sMensagem);

            //Configurações do SMTP
            string sUserName = "email@gmail.com"; //Login: email do gmail.
            string sPassword = "senha"; //Senha: senha do email do gmail.

            MailMessage objEmail = new MailMessage();
            objEmail.To.Add("contato@cbsa.com.br");
            objEmail.From = new MailAddress("contato@cbsa.com.br");
            objEmail.Subject = "Título da mensagem";
            objEmail.Body = sTemplate; //Inserindo o template

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com"; //Servidor smtp do gmail.
            smtp.Credentials = new NetworkCredential(sUserName, sPassword);
            smtp.EnableSsl = true;
            smtp.Send(objEmail);
        }

        public void EnviarEmail(string recepientEmail, string subject, string body)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(recepientEmail));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["Host"];
                smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];
                NetworkCred.Password = ConfigurationManager.AppSettings["Password"];
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                smtp.Send(mailMessage);
            }
        }

        public static string GerarSenhaRandomica(int quantidadeDeCaracteres)
        {
            var chars = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, quantidadeDeCaracteres)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        //Método para validar Email com Expressões Regulares(Regular Expressions)
        public static bool ValidarEmail(string email)
        {
            Regex regExpEmail = new Regex("^[A-Za-z0-9](([_.-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([.-]?[a-zA-Z0-9]+)*)([.][A-Za-z]{2,4})$");
            Match match = regExpEmail.Match(email);

            if (match.Success)
                return true;
            else
                return false;
        }

        //Valida CPF
        public static bool ValidarCPF(string CPF)
        {
            int[] mt1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] mt2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string TempCPF;
            string Digito;
            int soma;
            int resto;

            CPF = CPF.Trim();
            CPF = CPF.Replace(".", "").Replace("-", "");

            if (CPF.Length != 11)
                return false;

            TempCPF = CPF.Substring(0, 9);
            soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(TempCPF[i].ToString()) * mt1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            Digito = resto.ToString();
            TempCPF = TempCPF + Digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(TempCPF[i].ToString()) * mt2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            Digito = Digito + resto.ToString();

            return CPF.EndsWith(Digito);
        }

        public static string Criptografar(string Message, string senha)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(senha));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToEncrypt = UTF8.GetBytes(Message);
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return Convert.ToBase64String(Results);
        }

        public static string Descriptografar(string Message, string senha)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(senha));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(Message);
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return UTF8.GetString(Results);
        }
    }
}
