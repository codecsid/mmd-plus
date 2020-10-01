using DataAccess.Data.Abstract;
using DataAccess.Data.Migrations;
using DataAccess.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Data.Services
{
    public class GameControllerService : IGameControllerService
    {
        private readonly DataContext _ctx;
        private readonly ILogger<GameControllerService> _logger;

        public GameControllerService(DataContext ctx, ILogger<GameControllerService> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }


        public void AddNewGame(GameModel currentGame)
        {
            try
            {
                _ctx.Games.Add(new Model.Game
                {
                    GameId = currentGame.GameId,
                    TimeStamp = DateTime.UtcNow,
                });

                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while adding game to database : ", ex);
            }
        }

        public void AddNewRound(RoundConfig round, GameModel game)
        {
            try
            {

                var currentGame = _ctx.Games.Where(g => g.GameId == game.GameId).SingleOrDefault();

                if (currentGame != null)
                {

                    var newRound = new Round
                    {
                        RoundNumber = (int)round.Id,
                    };

                    currentGame.Rounds.Add(newRound);

                    if (round.Id > 1)
                    {
                        //Method to copy prev Round score to current round
                    }
                }
                else
                {
                    throw new Exception("No current game found for adding round");
                }
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while addng round: " ,ex);
            }
        }

        public void AddNewPhase(RoundConfig round, PhaseType phase, DateTime TimeStamp, Guid gameId)
        {
            var currRound = _ctx.Rounds.Where(r => (r.GameId == gameId) && (r.RoundNumber == (int)round.Id)).SingleOrDefault();

            if (currRound != null)
            {
                currRound.RoundPhases.Add(new Phase
                {
                    PhaseType = phase,
                    TimeStamp = TimeStamp,
                    GameId = gameId

                });

                _ctx.SaveChanges();
            }
        }

        public void EndGame(GameModel currGame)
        {
            var currentGameInstance = _ctx.Games.Where(g => g.GameId == currGame.GameId).SingleOrDefault();
            //EndGame
            Console.WriteLine("current Game ended");
        }

        public void CollectPenalty(RoundConfig roundConfig)
        {
            //collect penalty from participants
            //Console.WriteLine("penalty collected");


        }

    }
}
