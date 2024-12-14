using System.Net;
using System.Net.Mail;

namespace ADUserGroupManager
{
    public static class EmailSender
    {
        public static void SendEmail(string subject, string body)
        {
            var fromAddress = new MailAddress(Properties.Settings.Default.EmailFrom);
            var toAddress = new MailAddress(Properties.Settings.Default.EmailTo);
            string smtpServer = Properties.Settings.Default.SmtpServer;

            // Conversión del puerto
            int smtpPort = int.Parse(Properties.Settings.Default.SmtpPort);

            string username = Properties.Settings.Default.EmailUsername;
            string password = Properties.Settings.Default.EmailPassword;

            var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            smtpClient.Send(message);
        }
    }
}
