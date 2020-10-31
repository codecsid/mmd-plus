using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Abstract;
using DataAccess.Data.Services;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameApi.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = "BasicAuth")]
    public class JoinController : Controller
    {
        private readonly RequestContext requestContext;
        private readonly IGameApiService gameApiService;
        private readonly IRequestLoggingService requestLoggingService;
        private readonly ILogger<JoinController> logger;

        public JoinController(RequestContext requestContext, IGameApiService gameApiService, IRequestLoggingService requestLoggingService, ILogger<JoinController> logger)
        {
            this.requestContext = requestContext;
            this.gameApiService = gameApiService;
            this.requestLoggingService = requestLoggingService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<JsonResult> Post()
        {
            JsonResult response;
            try
            {
                JoinResponse joinResponse = await gameApiService.JoinCurrentRound(User.Identity.Name);

                joinResponse.RequestId = requestContext.RequestId;

                response = new JsonResult(joinResponse)
                {
                    StatusCode = 200
                };
            }
            catch (Exception)
            {
                logger.LogError($"Error Occurred while fetching current gamestatus");

                response = new JsonResult(new JoinResponse
                {
                    RequestId = requestContext.RequestId,
                    Err = new Error
                    {
                        Message = "Internal Server Error",
                        Description = "Server Failed to fetch Current Gamestatus"
                    },
                    Data = new JoinResponseData { Joined = false }
                })
                {
                    StatusCode = 500
                };
            }

            return response;
        }
    }
}
