using System;
using System.Net;
using System.Threading.Tasks;
using DataAccess.Data.Abstract;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using GameApi.Service.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameApi.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GameStatusController : Controller
    {
        private readonly RequestContext requestContext;
        private readonly IGameApiService gameApiService;
        private readonly ILogger<GameStatusController> logger;
        private readonly IRequestLoggingService requestLoggingService;

        public GameStatusController(RequestContext requestContext, IGameApiService gameApiService, IRequestLoggingService requestLoggingService, ILogger<GameStatusController> logger)
        {
            this.requestContext = requestContext ?? throw new ArgumentNullException("RequestContext");
            this.gameApiService = gameApiService ?? throw new ArgumentNullException("IGameApiService");
            this.requestLoggingService = requestLoggingService ?? throw new ArgumentNullException("IRequestLoggingService");
            this.logger = logger ?? throw new ArgumentNullException("ILogger");
        }

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            JsonResult response;
            try
            {
                GameStatusResponse gameStatus = await gameApiService.GetCurrentStatus();

                gameStatus.RequestId = requestContext.RequestId;

                response = new JsonResult(gameStatus)
                {
                    StatusCode = 200
                };
            }
            catch(Exception)
            {
                logger.LogError($"Error Occurred while fetching current gamestatus");

                response = new JsonResult(new GameStatusResponse
                {
                    RequestId = requestContext.RequestId,
                    Err = new Error
                    {
                        Message = "Internal Server Error",
                        Description = "Server Failed to fetch Current Gamestatus"
                    }
                })
                {
                    StatusCode = 500
                };
            }

            return response;
        }
    }
}