using System;
using System.Collections;
using System.Collections.Generic;

namespace DataAccess.Model
{
    public class Team : IEntityBase
    {
        public Team()
        {
            Users = new HashSet<User>();
            KillRecord = new HashSet<Kill>();
            DeathRecord = new HashSet<Kill>();
            ParticipationRecord = new HashSet<Participant>();
            ThrottledRequests = new HashSet<ThrottledRequest>();
            Guesses = new HashSet<Guess>();
            Scores = new HashSet<Score>();
            Requests = new HashSet<Request>();
        }
        public long Id { get; set; }
        public string TeamId { get; set; }
        public string SecretToken { get; set; }
        public string Location { get; set; }
        public bool IsRobot { get; set; }
        public Location LocationNav { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Kill> DeathRecord { get; set; }
        public ICollection<Kill> KillRecord { get; set; }
        public ICollection<Participant> ParticipationRecord { get; set; }
        public ICollection<Guess> Guesses { get; set; }
        public ICollection<Score> Scores { get; set; }
        public ICollection<Request> Requests { get; set; }
        public ICollection<ThrottledRequest> ThrottledRequests { get; set; }
    }
}