using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Model.SharedModels;

namespace mmd_plus.Models
{
    public class LiveViewModel
    {
        public Guid? GameId { get; set; }
        public Guid? RoundId { get; set; }
        public long? RoundNumber { get; set; }
        public string Phase { get; set; }
        public int? SecretLength { get; set; }
        public long JoiningDuration { get; set; }
        public long RunningDuration { get; set; }
        public long FinishedDuration { get; set; }
        public DateTime PhaseStartTime { get; set; }
        public List<LiveParticipantDetails> Participants_CurrentScore { get; set; }
        public List<LiveParticipantDetails> Participants_TotalScore { get; set; }

    }

}
