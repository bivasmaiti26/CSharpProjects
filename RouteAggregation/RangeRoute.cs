using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteAggregation
{
    class RangeRoute
    {
        public long integerIp { get; set; }
        public long upper { get; set; }
        public long lower { get; set; }
        public short highSegment { get; set; }
        public string path { get; set; }
    }
}
