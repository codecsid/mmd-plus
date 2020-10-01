using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model
{
    public class GameModel
    {
        public Guid GameId { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<RoundConfig> Rounds { get; set; }
    }
}
