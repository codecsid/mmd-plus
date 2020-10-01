using System;

namespace DataAccess.Model
{
    public class FutureGame : IEntityBase
    {
        public long Id { get; set; }
        public DateTime StartTime { get; set; }
        public bool RunOnce { get; set; }
        public DateTime RunGamesTill { get; set; }
    }
}