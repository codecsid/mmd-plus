using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DataAccess.Data.Abstract;
using DataAccess.Model;
using DataAccess.Model.SharedModels;
using Fare;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SendGrid;

namespace DataAccess.Data.Services
{
    public class LiveService : ILiveService
    {

        private readonly DataContext _context;
        private readonly ILogger<LiveService> _logger;

        public LiveService(DataContext context, ILogger<LiveService> logger)
        {
            _context = context ?? throw new ArgumentNullException("DataContext");
            _logger = logger ?? throw new ArgumentNullException("DataContext");
        }

        public virtual async Task<LiveResponse> GetLiveStatus() {

            try
            {
                LiveResponse response = new LiveResponse();

                var currentPhase = await _context.Phases.Include(p => p.Round).OrderByDescending(p => p.TimeStamp).FirstOrDefaultAsync();
                if (currentPhase == null)
                {
                    _logger.LogError("Error : Game not running");
                }
                else
                {
                    var roundConfig = await _context.RoundConfigs.Where(rc => rc.Id == currentPhase.Round.RoundNumber).SingleOrDefaultAsync();

                    var participants_round = await _context.Participants.Include(p => p.Team).Where(p => p.RoundId.CompareTo(currentPhase.RoundId) == 0).ToDictionaryAsync(p => p.TeamId);
                    
                    var participants_total = await _context.Participants.Include(p => p.Team).Where(p => p.GameId.CompareTo(currentPhase.GameId) == 0)
                                                .Select(p => new { p.TeamId, p.Team })
                                                .Distinct()
                                                .ToDictionaryAsync(p => p.TeamId)
                                                ;
                                                

                    var tScore = await _context.Scores.Where(s => s.GameId.CompareTo(currentPhase.GameId) == 0)
                                           .GroupBy(s => s.TeamId)
                                           .Select(s => new { TeamId = s.Key, TotalScore = s.Sum(p => p.PointsScored) })
                                           .ToDictionaryAsync(kg => kg.TeamId);

                    var cScore = await _context.Scores.Where(s => s.RoundId.CompareTo(currentPhase.RoundId) == 0)
                                           .GroupBy(s => s.TeamId)
                                           .Select(s => new { TeamId = s.Key, CurrentScore = s.Sum(p => p.PointsScored) })
                                           .ToDictionaryAsync(kg => kg.TeamId);

                    var killNumber = await (from k in _context.Kills.Where(k => k.RoundId.CompareTo(currentPhase.RoundId) == 0)
                                            group k by k.VictimId into killGroup
                                            select new
                                            {
                                                TeamId = killGroup.Key,
                                                TotalKills = killGroup.Count()
                                            }).ToDictionaryAsync(kg => kg.TeamId);

                    var participantsList_CurrentScore = new List<LiveParticipantDetails>();

                    var participantsList_TotalScore = new List<LiveParticipantDetails>();


                    foreach (var key in participants_total.Keys)
                    {
                        
                        bool inScore = tScore.ContainsKey(key);


                        participantsList_TotalScore.Add(new LiveParticipantDetails
                        {
                            TeamId = key,
                            Score = inScore ? tScore[key].TotalScore : 0,
                            IsRobot = participants_total[key].Team.IsRobot,
                            Location = participants_total[key].Team.Location,
                            RoundId = currentPhase.Round.RoundId,
                            GameId = currentPhase.Round.GameId,
                            RoundNumber = currentPhase.Round.RoundNumber,
                            Phase = currentPhase.PhaseType.ToString()
                        });
                    }

                    foreach (var key in participants_round.Keys)
                    {

                        bool inScore = cScore.ContainsKey(key);
                        int LifelinesRemaining = roundConfig.LifeLines - (killNumber.ContainsKey(key) ? killNumber[key].TotalKills : 0);
                        bool zeroLifelines = LifelinesRemaining <= 0 ? true : false;

                        participantsList_CurrentScore.Add(new LiveParticipantDetails
                        {
                            TeamId = key,
                            Score = inScore ? cScore[key].CurrentScore : 0,
                            IsAlive = participants_round[key].IsAlive,
                            IsRobot = participants_round[key].Team.IsRobot,
                            Location = participants_round[key].Team.Location,
                            Lifelines = zeroLifelines ? 0 : LifelinesRemaining,
                            RoundId = currentPhase.Round.RoundId,
                            GameId = currentPhase.Round.GameId,
                            RoundNumber = currentPhase.Round.RoundNumber,
                            Phase = currentPhase.PhaseType.ToString()
                        }) ;
                    }


                    response.RoundId = currentPhase.Round.RoundId;
                    response.GameId = currentPhase.Round.GameId;
                    response.RoundNumber = currentPhase.Round.RoundNumber;
                    response.SecretLength = roundConfig?.SecretLength;
                    response.Participants_Current = participantsList_CurrentScore;
                    response.Participants_Total = participantsList_TotalScore;
                    response.Phase = currentPhase.PhaseType.ToString();
                    response.JoiningDuration = roundConfig.JoiningDuration;
                    response.RunningDuration = roundConfig.RunningDuration;
                    response.FinishedDuration = roundConfig.FinishedDuration;
                    response.PhaseStartTime = currentPhase.TimeStamp;

                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fetching current gamestatus failed, {ex}");
                throw new ServerSideException("Failed in GetCurrentStatus()", ex);
            }

            
        }

    }
}
