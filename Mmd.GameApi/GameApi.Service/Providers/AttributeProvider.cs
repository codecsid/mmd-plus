using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameApi.Service.Providers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ThrottleAttribute : Attribute
    {
        public long ticks;

        private readonly long default_ticks = 5 * 10000000; // 5 seconds = 5 * 10^7 ticks
        public ThrottleAttribute()
        {
            ticks = default_ticks;
        }

        public ThrottleAttribute(long ticks)
        {
            this.ticks = ticks > 0 ? ticks : default_ticks;
        }

        public ThrottleAttribute(TimeSpan span)
        {
            ticks = span.Ticks > 0 ? span.Ticks : default_ticks;
        }
    }
}
