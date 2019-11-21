using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SendMail
{
    public class SendMessage : ActionFramework.Action
    {
        public string ApiKey { get; set; }
        public string Domain { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }

        public int Port { get; set; }

        public string Host { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }



        /// <summary>
        ///     {
	    ///         "subject": "my subject",
        ///         "message": my message
        ///         "recipents": "claes-philip@staiger.se,claes-philip.staiger@hiq.se"
        ///     }
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object Run(dynamic obj)
        {
            var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("test@example.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
            return response;
        }

        
    }
}
