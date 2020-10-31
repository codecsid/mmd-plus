using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model
{
    public class ThrottledRequest : IEntityBase
    {
        public long Id { get; set; }
        public string HitId { get; set; }
        public string Path { get; set; }
        public string TeamId { get; set; }
        public DateTime? LastHit { get; set; }
        public Team Team { get; set; }
    }
}
