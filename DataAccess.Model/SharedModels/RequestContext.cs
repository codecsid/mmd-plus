using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model.SharedModels
{
    public class RequestContext
    {
        public RequestContext()
        {
            RequestId = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }
        public Guid RequestId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
