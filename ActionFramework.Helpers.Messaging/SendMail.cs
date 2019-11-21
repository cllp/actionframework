using System;
using System.Collections.Generic;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace ActionFramework.Helpers.Messaging
{
    public class SendMail
    {
        private string apiKey = string.Empty;
        private SendGridClient client;
        private EmailAddress from;

        public SendMail(string apiKey, string emailFrom)
        {
            this.apiKey = apiKey;
            client = new SendGridClient(apiKey);
            from = new EmailAddress(emailFrom);
        }

        public async Task<SendGrid.Response> Send(string to, string subject, string textContent, string htmlContent)
        {
            var m_subject = subject;
            var m_to = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, m_to, m_subject, textContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<SendGrid.Response> Send(List<string> to, string subject, string textContent, string htmlContent)
        {
            List<EmailAddress> adressList = new List<EmailAddress>();

            foreach (var adress in to)
                adressList.Add(new EmailAddress(adress));

            var m_subject = subject;
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, adressList, m_subject, textContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}
