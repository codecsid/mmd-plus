using DataAccess.Data.Abstract;
using DataAccess.Data.Services;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameApi.Service.Middleware
{
    public class RequestRecordingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestRecordingMiddleware(RequestDelegate requestDelegate)
        {
            _next = requestDelegate ?? throw new ArgumentNullException("requestDelegate");
        }

        public async Task InvokeAsync(HttpContext context, RequestContext requestContext, ILogger<RequestRecordingMiddleware> logger, IRequestLoggingService requestLoggingService)
        {
            await _next(context);

            try
            {
                var requestMethod = (RequestMethod)Enum.Parse(typeof(RequestMethod), context.Request.Method);
                var statusCode = context.Response.StatusCode;
                var requestApi = context.Request.Path;
                logger.LogWarning($"Recording Request { requestContext.RequestId } | {requestMethod} | {requestApi} | {statusCode}");
                await requestLoggingService.RecordRequest(requestMethod, statusCode, requestApi, context.User.Identity.Name);
            }
            catch(Exception)
            {
                logger.LogError($"Failed recording request {requestContext.RequestId} | {context.Request.Method} | {context.Request.Path}");
            }
        }
    }
}
