using DataAccess.Data.Abstract;
using DataAccess.Model;
using EllipticCurve;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Mmd.GameController
{
    public class GameInstance : IGameInstance
    {

        private GameModel currentGame { get; set; }
        private string currentState;
        private readonly ILogger<GameInstance> _logger;
        private readonly IGameControllerService _svc;
        //private Timer _timer;
        private IOptions<AppSettings> _config;
        //private long _previousRoundId;

        public GameInstance(ILogger<GameInstance> logger, IGameControllerService svc, IOptions<AppSettings> config)
        {
            _logger = logger ?? throw new ArgumentNullException("logger");
            _svc = svc ?? throw new ArgumentNullException("svc");
            _config = config ?? throw new ArgumentNullException("config");
        }

        public void Initialize()
        {
            currentGame = new GameModel
            {
                GameId = Guid.NewGuid(),
                TimeStamp = DateTime.UtcNow,
                Rounds = _config.Value.Rounds

            };

            //log
            _logger.LogWarning("----------------------------------------");
            _logger.LogWarning($"{DateTime.UtcNow.ToLongTimeString()} Creating a new game : {currentGame.GameId}");
            _logger.LogWarning("----------------------------------------");

            //Add the game to the database
            _svc.AddNewGame(currentGame);

            RunGame().Wait();
        }

        private async Task RunGame()
        {
            //_previousRoundId = null;

            foreach (var round in currentGame.Rounds)
            {
                //Add round to Database
                _svc.AddNewRound(round, currentGame);

                //Start async timer for Joining phase
                Task phaseTimer = StartPhaseTimer(round.JoiningDuration);

                //Start joining phase
                StartJoinPhase(round);

                await phaseTimer;

                //Start async timer for Running Phase
                phaseTimer = StartPhaseTimer(round.RunningDuration);

                //Start Running phase
                StartRunningPhase(round);

                await phaseTimer;

                //Start async timer for Finished Phase
                phaseTimer = StartPhaseTimer(round.FinishedDuration);

                //Start Finished phase
                StartFinishPhase(round);

                if (round.Id == currentGame.Rounds.Count)  //Round numbers must start from 1
                    _svc.EndGame(currentGame);

                await phaseTimer;
            }

         }

        private async Task StartPhaseTimer(long phaseSpan)
        {
            await Task.Delay((int)phaseSpan);
        }

        private void StartJoinPhase(RoundConfig round)
        {
            _svc.AddNewPhase(round, PhaseType.Joining, DateTime.UtcNow, currentGame.GameId);

            currentState = "joining";
            string msg = $"{DateTime.UtcNow.ToLongTimeString()} {currentGame.GameId} RoundNumber: {round.Id} : {currentState}";

            _logger.LogWarning(msg);
        }

        private void StartRunningPhase(RoundConfig round)
        {
            _svc.AddNewPhase(round, PhaseType.Running, DateTime.UtcNow, currentGame.GameId);

            currentState = "running";
            string msg = $"{DateTime.UtcNow.ToLongTimeString()} {currentGame.GameId} RoundNumber: {round.Id} : {currentState}";

            _logger.LogWarning(msg);
        }

        private void StartFinishPhase(RoundConfig round)
        {
            _svc.AddNewPhase(round, PhaseType.Finished, DateTime.UtcNow, currentGame.GameId);

            currentState = "finished";
            string msg = $"{DateTime.UtcNow.ToLongTimeString()} {currentGame.GameId} RoundNumber: {round.Id} : {currentState}";

            _logger.LogWarning(msg);

            //Add penalty calculation
            _svc.CollectPenalty(round);
        }

    }
}
