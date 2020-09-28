﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccess.Data.Abstract;
using DataAccess.Data.Services;
using DataAccess.Model.SharedModels;
using GameApi.Service.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameApi.Service.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "BasicAuth")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Throttle]
    public class GuessController : Controller
    {
        private readonly RequestContext requestContext;
        private readonly IGameApiService gameApiService;
        private readonly ILogger<GuessController> logger;
        private readonly EvaluationModule evaluationModule;

        public GuessController(RequestContext requestContext, IGameApiService gameApiService, ILogger<GuessController> logger, EvaluationModule evaluationModule)
        {
            this.requestContext = requestContext ?? throw new ArgumentNullException("RequestContext");
            this.gameApiService = gameApiService ?? throw new ArgumentNullException("GameApiService");
            this.logger = logger ?? throw new ArgumentNullException("Logger");
            this.evaluationModule = evaluationModule ?? throw new ArgumentNullException("Logger");
        }

        [HttpPost]
        public async Task<JsonResult> Post([FromBody] GuessRequestBody requestBody)
        {
            JsonResult response;

            try
            {
                GuessResponse guessResponse = new GuessResponse()
                {
                    RequestId = requestContext.RequestId
                };

                if (!await gameApiService.IsCurrentPhaseRunning())
                    throw new GameNotInRunningPhaseException();

                if (!await gameApiService.IsTeamAlive(User.Identity.Name))
                    throw new TeamNotJoinedException();

                var guesses = requestBody.Guesses;

                var body = PreProcess(requestBody);

                var validatedBody = Validate(body);

                await Evaluate(validatedBody);

                guessResponse.Data.Guesses = validatedBody.Guesses;
                
                response = new JsonResult(guessResponse)
                {
                    StatusCode = 200
                };
            }
            catch(TeamNotJoinedException)
            {
                response = new JsonResult(new GuessResponse
                {
                    RequestId = requestContext.RequestId,
                    Err = new Error
                    {
                        Message = "Team not alive or joined",
                        Description = "Either you have not joined this round or your team is dead."
                    }
                })
                {
                    StatusCode = 400
                };
            }
            catch(InvalidDataProvidedException)
            {
                response = new JsonResult(new GuessResponse
                {
                    RequestId = requestContext.RequestId,
                    Err = new Error
                    {
                        Message = "Invalid Request Body",
                        Description = "Please provide a proper body for the api"
                    }
                })
                {
                    StatusCode = 400
                };
            }
            catch (Exception)
            {
                logger.LogError($"Error Occurred while fetching current gamestatus");

                response = new JsonResult(new GuessResponse
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

        private async Task Evaluate(GuessResponseBody validatedBody)
        {
            foreach(var guess in validatedBody.Guesses)
            {
                if(guess.IsValid)
                {
                    bool targetTeamIsAlive = await gameApiService.IsTeamAlive(guess.TargetTeam);

                    if (!targetTeamIsAlive)
                    {
                        guess.IsValid = false;
                        guess.ErrMessage = "Target Team is Dead";
                    }
                    else
                    {
                        var evaluationResult = await evaluationModule.EvaluateTheGuess(guess.TargetTeam, guess.Guess);

                        guess.NoOfDigitsMatchedByPositionAndValue = evaluationResult.NoOfDigitsMatchedByValueAndPosition;
                        guess.NoOfDigitsMatchedByValue = evaluationResult.NoOfDigitsMatchedByValue;
                        guess.Score = evaluationResult.PointsScored;
                    }
                }
            }
        }

        private GuessResponseBody Validate(GuessRequestBody body)
        {
            var guesses = body.Guesses;
            var guessResponseBody = new GuessResponseBody { Guesses = new List<SingleGuessResponseObject>() };
            foreach(var guess in guesses)
            {
                var targetTeam = guess.Team;
                var secretGuess = guess.Guess;
                var guessResponseObject = new SingleGuessResponseObject();

                if(string.IsNullOrWhiteSpace(targetTeam) || !IsTeamRegexValid(targetTeam))
                {
                    guessResponseObject.ErrMessage = "Invalid team";
                }
                else if(string.IsNullOrWhiteSpace(secretGuess) || !IsGuessRegexValid(secretGuess))
                {
                    guessResponseObject.ErrMessage = "Invalid guess";
                }

                if (string.IsNullOrEmpty(guessResponseObject.ErrMessage))
                    guessResponseObject.IsValid = true;

                guessResponseObject.TargetTeam = targetTeam;
                guessResponseObject.Guess = secretGuess;

                guessResponseBody.Guesses.Add(guessResponseObject);
            }

            return guessResponseBody;
        }

        private bool IsGuessRegexValid(string secretGuess)
        {
            return new Regex("^[0-9]+$").IsMatch(secretGuess);
        }

        private bool IsTeamRegexValid(string targetTeam)
        {
            return new Regex("^(?=[a-z0-9._]{5,20}$)(?=.*[a-z]+.*)(?!.*[_.]{2})[^_.].*[^_.]$").IsMatch(targetTeam);
        }

        private GuessRequestBody PreProcess(GuessRequestBody requestBody)
        {
            if (requestBody == null || requestBody.Guesses == null)
                throw new InvalidDataProvidedException($"Found Invalid Data in GuessRequestBody {requestContext.RequestId}");

            var newModel = new GuessRequestBody();

            newModel.Guesses = new List<SingleGuessRequestObject>();

            foreach(var guess in requestBody.Guesses)
            {
                newModel.Guesses.Add(new SingleGuessRequestObject
                {
                    Team = guess.Team.ToLowerInvariant(),
                    Guess = guess.Guess.ToLowerInvariant()
                });
            }

            return newModel;
        }
    }
}
