using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace APS.Services
{
    public class Service
    {
        public static Task SendMail(string Destination, string Subject, string Body, string path = "", string cc = "", bool bypassDebugMode = false)
        {
            IConfigurationRoot configuration = Methods.GlobalMethods.GetConfiguration();
            string mailServer = configuration.GetSection("Mail").GetValue(typeof(string), "Server").ToString();
            string mailfrom = configuration.GetSection("Mail").GetValue(typeof(string), "From").ToString();
            string debug = configuration.GetSection("Mail").GetValue(typeof(string), "Debug").ToString();
            if (debug != "off" && !bypassDebugMode)
                Destination = debug;

            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = mailServer;
            client.EnableSsl = false;
            //client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            MailMessage m = new MailMessage(mailfrom, Destination, Subject, Body);
            if (!string.IsNullOrWhiteSpace(cc))
                m.CC.Add(cc);
            m.IsBodyHtml = true;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                m.Attachments.Add(new Attachment(path));
            return client.SendMailAsync(m);
        }
    }
}
