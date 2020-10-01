using System;

namespace DataAccess.Model
{
    public class Score : IEntityBase
    {
        public long Id { get; set; }
        public Guid GameId { get; set; }
        public Guid RoundId { get; set; }
        public string TeamId { get; set; }
        public Guid? GuessId { get; set; }
        public long PointsScored { get; set; }
        public DateTime TimeStamp { get; set; }
        public Game Game { get; set; }
        public Round Round { get; set; }
        public Team Team { get; set; }
        public Guess Guess { get; set; }
    }
}