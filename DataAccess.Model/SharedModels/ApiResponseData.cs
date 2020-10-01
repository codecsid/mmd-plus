using System;
using DataAccess.Model;
using Newtonsoft.Json;

namespace DataAccess.Model.SharedModels
{
    public abstract class ApiResponseData
    {
        public ApiResponseData()
        {
            GameId = null;
            RoundId = null;
            RoundNumber = null;
            Status = null;
        }
        [JsonProperty(Order = 1)]
        public Guid? GameId { get; set; }
        [JsonProperty(Order = 2)]
        public Guid? RoundId { get; set; }
        [JsonProperty(Order = 3)]
        public long? RoundNumber { get; set; }
        [JsonProperty(Order = 4)]
        public string Status { get; set; }
    }
}