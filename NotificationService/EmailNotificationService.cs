using DataAccess.Model.SharedModels;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace NotificationService
{
    public class EmailNotificationService : INotificationService
    {
        private SmtpClient Client { get; set; }
        private SendGridClient sgClient { get; set; }

        public EmailNotificationService(SendGridClient sgClient)
        {
            this.sgClient = sgClient;
        }

        public bool Notify(NotificationContent content)
        {
            try {
                var from = new EmailAddress("singh.shraddhesh@gmail.com", "Shraddhesh Singh");
                var subject = content.Subject;
                var recieverList = new List<EmailAddress>();
                foreach(var email in content.Recievers){
                    recieverList.Add(new EmailAddress(email));
                }
                var plainTextContent = content.Body;
                var htmlContent = $"<strong>{content.Body}</strong>";
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, recieverList, subject, plainTextContent, htmlContent);
                
                return SendEmail(msg).Result;

            }
            catch(Exception){
                return false;
            }
        }

        private async Task<bool> SendEmail(SendGridMessage msg)
        {
            try{
                var response = await sgClient.SendEmailAsync(msg).ConfigureAwait(false);

                if(response.StatusCode < System.Net.HttpStatusCode.BadRequest)
                    return true;

                return false;
            }    
            catch(Exception){
                return false;
            }
        }
    }
}
