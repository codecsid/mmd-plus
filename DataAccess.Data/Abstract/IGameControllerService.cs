using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Data.Abstract
{
    public interface IGameControllerService
    {

        void AddNewGame(GameModel currentGame);
        void AddNewRound(RoundConfig round, GameModel game);
        void AddNewPhase(RoundConfig round, PhaseType phase, DateTime timeStamp, Guid gameId);
        void EndGame(GameModel currentGame);
        void CollectPenalty(RoundConfig round);
    }
}
