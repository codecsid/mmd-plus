using System;

namespace DataAccess.Model
{
    public class Participant : IEntityBase
    {
        public long Id { get; set; }
        public Guid GameId { get; set; }
        public Guid RoundId { get; set; }
        public string TeamId { get; set; }
        public string Secret { get; set; }
        public bool? IsAlive { get; set; }
        public DateTime JoinedAt { get; set; }
        public Game Game { get; set; }
        public Round Round { get; set; }
        public Team Team { get; set; }
    }
}