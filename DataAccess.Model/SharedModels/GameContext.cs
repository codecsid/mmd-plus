using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model.SharedModels
{
    public class GameContext
    {
        public Guid GameId { get; set; }
        public Guid RoundId { get; set; }
        public PhaseType CurrentPhase { get; set; }
        public long RoundNumber { get; set; }
        public List<Participant> Participants { get; set; }
    }
}
