using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Model;
using Newtonsoft.Json;

namespace DataAccess.Model.SharedModels
{
    public class LiveResponse
    {
        public LiveResponse()
        {
            GameId = null;
            RoundId = null;
            RoundNumber = null;
            Phase = null;
            SecretLength = null;
        }

        public Guid? GameId { get; set; }
        public Guid? RoundId { get; set; }
        public long? RoundNumber { get; set; }
        public string Phase { get; set; }
        public int? SecretLength { get; set; }
        public long JoiningDuration { get; set; }
        public long RunningDuration { get; set; }
        public long FinishedDuration { get; set; }
        public DateTime PhaseStartTime { get; set; }
        public List<LiveParticipantDetails> Participants_Current { get; set; }
        public List<LiveParticipantDetails> Participants_Total { get; set; }

    }
}
