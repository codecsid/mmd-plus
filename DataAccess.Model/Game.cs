using System.Collections.Generic;
using System;
namespace DataAccess.Model
{
    public class Game : IEntityBase
    {
        public Game()
        {
            Rounds = new HashSet<Round>();
            Phases = new HashSet<Phase>();
            Kills = new HashSet<Kill>();
            Participants = new HashSet<Participant>();
            Guesses = new HashSet<Guess>();
            Scores = new HashSet<Score>();
            Requests = new HashSet<Request>();
        }
        public long Id {get; set;}
        public Guid GameId {get; set;}
        public DateTime TimeStamp {get; set;}
        public ICollection<Round> Rounds {get; set;}
        public ICollection<Phase> Phases {get; set;}
        public ICollection<Kill> Kills { get; set; }
        public ICollection<Participant> Participants { get; set; }
        public ICollection<Guess> Guesses { get; set; }
        public ICollection<Score> Scores { get; set; }
        public ICollection<Request> Requests { get; set; }
    }
}