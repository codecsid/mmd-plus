using DataAccess.Data.Abstract;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Services
{
    public class RequestLoggingService : IRequestLoggingService
    {
        private readonly RequestContext requestContext;
        private readonly DataContext context;

        public RequestLoggingService(RequestContext requestContext, DataContext context)
        {
            this.requestContext = requestContext ?? throw new ArgumentNullException("RequestContext");
            this.context = context ?? throw new ArgumentNullException("Context");
        }
        public async Task RecordRequest(RequestMethod method, int? statusCode, string requestApi, string teamId)
        {
            var currentPhase = await context.Phases.OrderByDescending(p => p.TimeStamp).FirstOrDefaultAsync();

            var newRequest = new Request
            {
                GameId = currentPhase?.GameId,
                RoundId = currentPhase?.RoundId,
                RequestId = requestContext.RequestId,
                StatusCode = (HttpStatusCode)statusCode,
                TeamId = teamId,
                RequestApi = requestApi,
                RequestMethod = method,
                TimeStamp = requestContext.TimeStamp
            };

            await context.Requests.AddAsync(newRequest);

            await context.SaveChangesAsync();
        }
    }
}
