using System;
using Newtonsoft.Json;

namespace DataAccess.Model.SharedModels
{
    public abstract class ApiResponse<T> where T : ApiResponseData
    {
        [JsonProperty(Order = 1)]
        public Guid RequestId { get; set; }
        [JsonProperty(Order = 2)]
        public Error Err { get; set; }
        [JsonProperty(Order = 3)]
        public abstract T Data { get; set; }
    }

    public class GameStatusResponse : ApiResponse<GameStatusResponseData>
    {
        public override GameStatusResponseData Data { get; set; }
    }

    public class JoinResponse : ApiResponse<JoinResponseData>
    {
        public override JoinResponseData Data { get; set; }
    }

    public class GuessResponse : ApiResponse<GuessResponseData>
    {
        public override GuessResponseData Data { get; set; }
    }
}