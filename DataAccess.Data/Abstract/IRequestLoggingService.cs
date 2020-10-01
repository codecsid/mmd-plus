using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Abstract
{
    public interface IRequestLoggingService
    {
        Task RecordRequest(RequestMethod method, int? statusCode, string requestApi, string teamId);
    }
}
