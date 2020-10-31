using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model.SharedModels
{
    public class LiveParticipantDetails
    {

        public LiveParticipantDetails()
        {
            GameId = null;
            RoundId = null;
            RoundNumber = null;
            Phase = null;
            Lifelines = null;
            IsAlive = null;
        }


        public string TeamId { get; set; }
        public long Score { get; set; }
        public bool? IsAlive { get; set; }
        public bool IsRobot { get; set; }
        public string Location { get; set; }
        public int? Lifelines { get; set; }

        public Guid? GameId { get; set; }
        public Guid? RoundId { get; set; }
        public long? RoundNumber { get; set; }
        public string Phase { get; set; }


    }
}
