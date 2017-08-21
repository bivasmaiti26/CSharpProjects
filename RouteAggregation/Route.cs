using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteAggregation
{
    class Route
    {
        public string IPAddress {get; set;}
        public string Path { get; set; }
        public short Segment { get; set; }
        public long ipAddresstoInt()
        {
            long integerIp = 0;
            
            string[] parts = this.IPAddress.Split('.');
            if (parts.Length != 4)
            {
                return 0;
            }
            for (int i = 0; i < 4; i++)
            {
                int num = Convert.ToInt16(parts[i]);
                if (num >= 0 && num <= 255)
                {
                    integerIp += Convert.ToInt64(num * (Math.Pow(2, (8 * (3 - i)))));
                }
            }
            return integerIp;
        }
    }

}
