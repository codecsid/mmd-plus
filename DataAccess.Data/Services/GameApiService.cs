using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Abstract;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using Fare;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Data.Services
{
    public class GameApiService : IGameApiService
    {
        private readonly DataContext _context;
        private readonly ILogger<GameApiService> _logger;
        private readonly RequestContext _requestContext;
        private readonly GameContext _gameContext;

        public GameApiService(DataContext context, ILogger<GameApiService> logger, RequestContext requestContext, GameContext gameContext)
        {
            _context = context ?? throw new ArgumentNullException("DataContext");
            _logger = logger ?? throw new ArgumentNullException("DataContext");
            _requestContext = requestContext ?? throw new ArgumentNullException("RequestContext");
            _gameContext = gameContext ?? throw new ArgumentNullException("GameContext");
        }

        public virtual async Task<GameStatusResponse> GetCurrentStatus()
        {
            try
            {
                GameStatusResponse response = new GameStatusResponse()
                {
                    RequestId = _requestContext.RequestId
                };
                var currentPhase = await _context.Phases.Include(p => p.Round).OrderByDescending(p => p.TimeStamp).FirstOrDefaultAsync();
                if(currentPhase == null)
                {
                    response.Err = new Error
                    {
                        Message = "Try after some time",
                        Description = "Game is not running"
                    };
                }
                else
                {
                    var roundConfig = await _context.RoundConfigs.Where(rc => rc.Id == currentPhase.Round.RoundNumber).SingleOrDefaultAsync();
                    var participants = await _context.Participants.Include(p => p.Team).Where(p => p.RoundId.CompareTo(currentPhase.RoundId) == 0).ToDictionaryAsync(p => p.TeamId);
                    
                    var scores = await (from t in (from s in _context.Scores.Where(s => s.GameId.CompareTo(currentPhase.GameId) == 0)
                                                   group s by s.TeamId into scoreGroup
                                                   select new
                                                   {
                                                       TeamId = scoreGroup.Key,
                                                       TotalScore = scoreGroup.Sum(sg => sg.PointsScored)
                                                   })
                                        join c in (from k in _context.Scores.Where(s => s.GameId.CompareTo(currentPhase.GameId) == 0)
                                                   group k by new { k.TeamId, k.RoundId } into scoreGroup
                                                   select new
                                                   {
                                                       TeamId = scoreGroup.Key.TeamId,
                                                       scoreGroup.Key.RoundId,
                                                       CurrentScore = scoreGroup.Sum(sg => sg.PointsScored)
                                                   })
                                        on t.TeamId equals c.TeamId
                                        select new
                                        {
                                            TeamId = t.TeamId,
                                            RoundId = c.RoundId,
                                            CurrentScore = c.CurrentScore,
                                            TotalScore = t.TotalScore
                                        }).Where(q => q.RoundId.CompareTo(currentPhase.RoundId) == 0).ToDictionaryAsync(sg => sg.TeamId);

                    var participantsList = new List<TeamData>();
                    foreach(var key in participants.Keys)
                    {
                        participantsList.Add(new TeamData
                        {
                            TeamId = key,
                            CurrentScore = scores[key].CurrentScore,
                            TotalScore = scores[key].TotalScore,
                            IsAlive = participants[key].IsAlive,
                            IsRobot = participants[key].Team.IsRobot
                        });
                    }
                    response.Data = new GameStatusResponseData
                    {
                        RoundId = currentPhase.Round.RoundId,
                        GameId = currentPhase.Round.GameId,
                        RoundNumber = currentPhase.Round.RoundNumber,
                        SecretLength = roundConfig?.SecretLength,
                        Participants = participantsList,
                        Status = currentPhase.PhaseType.ToString()
                    };
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fetching current gamestatus failed, {ex}");
                throw new ServerSideException("Failed in GetCurrentStatus()", ex);
            }
        }

        public async Task<Phase> GetCurrentPhase()
        {
            return await _context.Phases.OrderByDescending(p => p.TimeStamp).FirstOrDefaultAsync();
        }

        public async Task<bool> IsCurrentPhaseRunning()
        {
            var currentPhase = await GetCurrentPhase();
            return currentPhase == null ? false : currentPhase.PhaseType == PhaseType.Running;
        }

        public async Task<bool> IsTeamAlive(string targetTeam)
        {
            var currentPhase = await GetCurrentPhase();
            return (await _context.Participants.Where(p => 
                p.TeamId == targetTeam && 
                p.IsAlive != null && 
                p.IsAlive == true &&
                p.RoundId.Equals(currentPhase.RoundId)    
            ).SingleOrDefaultAsync()) != null;
        }

        public virtual async Task<JoinResponse> JoinCurrentRound(string teamId)
        {
            JoinResponse response = new JoinResponse();
            if(string.IsNullOrWhiteSpace(teamId))
            {
                response.Err = new Error
                {
                    Message = "Invalid Username",
                    Description = "Failed to parse username from authorization header"
                };
                return response;
            }

            var currentPhase = await _context.Phases.Include(p => p.Round).OrderByDescending(p => p.TimeStamp).FirstOrDefaultAsync();

            if(currentPhase == null)
            {
                response.Err = new Error
                {
                    Message = "Try after some time",
                    Description = "Game is not running"
                };
            }
            else if(!currentPhase.PhaseType.Equals(PhaseType.Joining))
            {
                response.Err = new Error
                {
                    Message = "Failed to Join the Round",
                    Description = "Round is not in Joining phase"
                };
            }
            else
            {
                var existingParticipant = await _context.Participants.Where(p => p.TeamId == teamId.ToLowerInvariant() && p.RoundId.CompareTo(currentPhase.RoundId) == 0).SingleOrDefaultAsync();
                var roundConfig = await _context.RoundConfigs.Where(rc => rc.Id == currentPhase.Round.RoundNumber).SingleOrDefaultAsync();

                if (existingParticipant == null)
                {
                    await _context.Participants.AddAsync(new Participant
                    {
                        GameId = currentPhase.GameId,
                        RoundId = currentPhase.RoundId,
                        IsAlive = true,
                        TeamId = teamId.ToLowerInvariant(),
                        Secret = GenerateSecret(roundConfig?.SecretLength),
                        JoinedAt = DateTime.UtcNow
                    });
                    await _context.Scores.AddAsync(new Score
                    {
                        GameId = currentPhase.GameId,
                        RoundId = currentPhase.RoundId,
                        PointsScored = 0,
                        TimeStamp = DateTime.UtcNow,
                        TeamId = teamId.ToLowerInvariant()
                    });

                    await _context.SaveChangesAsync();
                }
                else
                {
                    response.Err = new Error
                    {
                        Message = "Can't Join Again",
                        Description = "You have already joined the round"
                    };
                }

                response.Data = new JoinResponseData { Joined = true };
            }

            if (response.Err != null)
                response.Data = new JoinResponseData { Joined = false };

            if (response.Data != null && currentPhase != null)
            {
                var responseData = response.Data;
                responseData.GameId = currentPhase.GameId;
                responseData.RoundId = currentPhase.RoundId;
                responseData.RoundNumber = currentPhase.Round.RoundNumber;
                responseData.Status = currentPhase.PhaseType.ToString();
            }

            return response;
        }

        private string GenerateSecret(int? secretLength)
        {
            if (secretLength == null)
                secretLength = 5;

            var regex = "^[0-9]{" + secretLength + "}$";

            var xeger = new Xeger(regex);
            return xeger.Generate();
        }

        public async Task<bool> ValidRequest(string path, string username, long ticks)
        {
            bool validRequest = true;
            path = path.ToLowerInvariant();
            username = username.ToLowerInvariant();
            var key = $"{username}-{path}";
            var existingRequest = await _context.ThrottledRequests.FindAsync(key);
            if (existingRequest == null)
            {
                await _context.ThrottledRequests.AddAsync(new ThrottledRequest
                {
                    HitId = key,
                    Path = path,
                    TeamId = username,
                    LastHit = _requestContext.TimeStamp
                });
            }
            else
            {
                var utcLastHit = TimeZoneInfo.ConvertTimeToUtc((DateTime)existingRequest.LastHit);

                if (_requestContext.TimeStamp - utcLastHit < new TimeSpan(ticks))
                    validRequest = false;
                else
                {
                    var existingRequestWithSameLastHit = await _context.ThrottledRequests.Where(tr => tr.HitId == key && tr.LastHit == existingRequest.LastHit).SingleOrDefaultAsync();
                    if(existingRequestWithSameLastHit != null)
                        existingRequestWithSameLastHit.LastHit = _requestContext.TimeStamp;
                }
            }
            await _context.SaveChangesAsync();
            return validRequest;
        }

        public async Task ApplyEvaluation(string guessingTeam, string targetTeam, long pointsScored, bool correctGuess)
        {
            var guessingTeamEntity = await _context.Participants.FindAsync(_gameContext.RoundId, guessingTeam);
            var targetTeamEntity = await _context.Participants.FindAsync(_gameContext.RoundId, targetTeam);

            if (guessingTeamEntity.IsAlive != null && guessingTeamEntity.IsAlive == false)
                throw new GuessingTeamDeadException();

            if (targetTeamEntity.IsAlive != null && targetTeamEntity.IsAlive == false)
                throw new TargetTeamDeadException();

            var targetTeamDeaths = await _context.Kills.Where(k => k.RoundId.CompareTo(_gameContext.RoundId) == 0 && k.VictimId == targetTeam).ToListAsync();

            var targetTeamDeathCount = targetTeamDeaths.Count();

            var alreadyKilledBySameTeam = targetTeamDeaths.Where(d => d.KillerId == guessingTeam).SingleOrDefault() != null;

            if (alreadyKilledBySameTeam)
                throw new TargetAlreadyKilledException();

            if (correctGuess)
            {
                var roundConfig = await _context.RoundConfigs.FindAsync(_gameContext.RoundNumber);
                if(roundConfig != null)
                {
                    if (targetTeamDeathCount == roundConfig.LifeLines)
                        throw new TargetTeamDeadException();

                    else if (targetTeamDeathCount < roundConfig.LifeLines)
                        await _context.Kills.AddAsync(new Kill
                        {
                            GameId = _gameContext.GameId,
                            RoundId = _gameContext.RoundId,
                            KillerId = guessingTeam,
                            VictimId = targetTeam,
                            TimeStamp = DateTime.UtcNow
                        });

                    if (targetTeamDeathCount + 1 == roundConfig.LifeLines)
                        targetTeamEntity.IsAlive = false;

                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task CommitGuess(string guessingTeam, GuessRequestBody requestBody, GuessResponseBody responseBody)
        {
            long pointsScored = 0;

            foreach (var guess in responseBody.Guesses)
                pointsScored += guess.Score;

            var guessId = Guid.NewGuid();

            await _context.Guesses.AddAsync(new Guess
            {
                GameId = _gameContext.GameId,
                RoundId = _gameContext.RoundId,
                GuessId = guessId,
                TeamId = guessingTeam,
                GuessRequest = requestBody,
                GuessResponse = responseBody,
                TimeStamp = DateTime.UtcNow
            });

            await _context.Scores.AddAsync(new Score
            {
                GameId = _gameContext.GameId,
                RoundId = _gameContext.RoundId,
                PointsScored = pointsScored,
                GuessId = guessId,
                TeamId = guessingTeam,
                TimeStamp = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
    }
}