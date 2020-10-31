using System;
using System.Collections.Generic;
using DataAccess.Model;
using Newtonsoft.Json;

namespace DataAccess.Model.SharedModels
{
    public class GameStatusResponseData : ApiResponseData
    {
        public GameStatusResponseData()
        {
            SecretLength = null;
        }

        [JsonProperty(Order = 5)]
        public int? SecretLength { get; set; }
        [JsonProperty(Order = 6)]
        public List<TeamData> Participants { get; set; }

    }
}