using System;

namespace DataAccess.Model
{
    public class Phase : IEntityBase
    {
        public Phase()
        {

        }

        public long Id {get; set;}
        public Guid GameId {get; set;}
        public Guid RoundId { get; set; }
        public PhaseType PhaseType {get; set;}
        public Round Round {get; set;}
        public Game Game {get; set;}
        public DateTime TimeStamp { get; set; }
        
    }

    public enum PhaseType
    {
        Joining,
        Running,
        Finished
    }
}