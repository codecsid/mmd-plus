using Newtonsoft.Json;

namespace DataAccess.Model.SharedModels
{
    public class TeamData
    {
        [JsonProperty(Order = 1)]
        public string TeamId { get; set; }
        [JsonProperty(Order = 2)]
        public long CurrentScore { get; set; }
        [JsonProperty(Order = 3)]
        public long TotalScore { get; set; }
        [JsonProperty(Order = 4)]
        public bool? IsAlive { get; set; }
        [JsonProperty(Order = 5)]
        public bool IsRobot { get; set; }
    }
}