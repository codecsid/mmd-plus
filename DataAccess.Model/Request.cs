using System;
using System.Net;

namespace DataAccess.Model
{
    public class Request : IEntityBase
    {
        public long Id { get; set; }
        public Guid? GameId { get; set; }
        public Guid? RoundId { get; set; }
        public Guid RequestId { get; set; }
        public string TeamId { get; set; }
        public RequestMethod RequestMethod { get; set; }
        public string RequestApi { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public DateTime TimeStamp { get; set; }
        public Game Game { get; set; }
        public Round Round { get; set; }
        public Team Team { get; set; }
    }

    public enum RequestMethod {
        GET,
        POST
    }

    public enum RequestApi {
        Gamestatus,
        Join,
        Guess
    }
}