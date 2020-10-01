using System;

namespace DataAccess.Model
{
    public class Kill : IEntityBase
    {
        public long Id { get; set; }
        public Guid RoundId { get; set; }
        public Guid GameId { get; set; }
        public string VictimId { get; set; }
        public string KillerId { get; set; }
        public DateTime TimeStamp { get; set; }
        public Round Round { get; set; }
        public Game Game { get; set; }
        public Team Victim { get; set; }
        public Team Killer { get; set; }
    }
}