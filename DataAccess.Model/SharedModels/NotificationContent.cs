using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model.SharedModels
{
    public class NotificationContent
    {
        public string Sender { get; set; }
        public List<string> Recievers { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> CcUsers { get; set; }
        public List<string> BccUsers { get; set; }
    }
}
