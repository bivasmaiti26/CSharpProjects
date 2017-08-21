using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteAggregation
{
    class Program
    {
        static void Main(string[] args)
        {
            Helper helper = new Helper();
            List<Route> routeTable= helper.GetDataFromFile();
            //string ip;
            //ip=Console.ReadLine();
            List<RangeRoute> rangeList=helper.MakeNewList(routeTable);
            helper.WriteTofile(rangeList);
           //// long add = helper.ipAddresstoInt(ip);
           //// long start, end;
           //// helper.GetRange(add, 22, out start, out end);
           // //long add1 = 0xff;
           // Console.WriteLine(add);
           // Console.WriteLine("Hex: {0:X}", add+1);
           // Byte bytedata=new Byte();
           // bytedata = Convert.ToByte("00000010", 2);
           // Console.WriteLine(bytedata);
            //Console.ReadLine();
             
        }
    }
}
