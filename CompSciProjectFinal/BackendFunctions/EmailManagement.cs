using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Net;
using System.Net.Http;

namespace CompSciProjectFinal
{
    public partial class EmailConfigurationDetails
    {
        public string email_address { get; set; }
        public string email_password { get; set; }
        public int email_server_port { get; set; }
        public string email_server { get; set; }
        public bool email_is_ssl { get; set; }
        
    }

    public partial class EmailMessages
    {
        public string order_confirm_email_text { get; set; }
        public string order_paid_email_text { get; set; }
        public string order_posted_email_text { get; set; }
    }

    public partial class SmtpClientWithSenderEmail
    {
        public SmtpClient smtpClient { get; set; }
        public string senderAddress { get; set; }
        
    }

    internal class EmailManagement //Class to handle the sending and management of emails. It is used in the sales process.
    {
        public static EmailConfigurationDetails GetEmailConfiguration() //Loads the email configuration json file and fills a configuration ovject with the data
        {
            //I would've encrypted the email data but according to the documentation for the builtin encryption method it won't work on home editions of windows. This is an unnecessarry restriction that I don't want my program to have.
            string fileText = File.ReadAllText(FileProcessing.LoadFile(ProgramConfigurationManagement.GetDataPath() + "/email_configuration.json"));
            return JsonSerializer.Deserialize<EmailConfigurationDetails>(fileText);
        }
        
        private static SmtpClientWithSenderEmail GetEmailDetails() //Get the email configuration details and return an smtpclient
        {
            EmailConfigurationDetails emailConfigurationDetails = GetEmailConfiguration(); //Get email ocnfig
            return new SmtpClientWithSenderEmail() //return smtpclient containing these details
            {
                smtpClient = new SmtpClient(emailConfigurationDetails.email_server) //smtpclient
                {
                    Port = emailConfigurationDetails.email_server_port,
                    Credentials = new NetworkCredential(emailConfigurationDetails.email_address, emailConfigurationDetails.email_password),
                    EnableSsl = emailConfigurationDetails.email_is_ssl
                },
                senderAddress = emailConfigurationDetails.email_address //return sender address
            };
        }

        public static EmailMessages GetEmailMessages() //Get the email messages from the confguration file
        {
            string fileText = File.ReadAllText(FileProcessing.LoadFile(ProgramConfigurationManagement.GetDataPath() + "/email_messages.json", true));
            return JsonSerializer.Deserialize<EmailMessages>(fileText);
        }

        public static void SendEmail(string recpient, string subject, string body) //send an email to a specified recpiient with a specified message
        {
            SmtpClientWithSenderEmail details = GetEmailDetails(); //get email client details
            details.smtpClient.Send(details.senderAddress, recpient, subject, body); //send message
        }

        public static void SaveEmailMessages(EmailMessages emailMessages) //save email sessages set by user to file
        {
            string fileText = JsonSerializer.Serialize(emailMessages);
            FileProcessing.SaveTextFile(fileText, ProgramConfigurationManagement.GetDataPath() + "/email_messages.json");
        }

        public static void SaveEmailConfiguration(EmailConfigurationDetails emailConfigurationDetails) //set email configuration set by use to file
        {
            string fileText = JsonSerializer.Serialize(emailConfigurationDetails);
            FileProcessing.SaveTextFile(fileText, ProgramConfigurationManagement.GetDataPath() + "/email_configuration.json");
        }
    }
}
