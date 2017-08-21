using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteAggregation
{
    class Helper
    {
        public List<Route> GetDataFromFile()
        {
            try
            {
                List<Route> routeTable = new List<Route>();
                //string fullPath = "C:\\Users\\Bivas\\Documents\\Visual Studio 2013\\Projects\\RouteAggregation\\RouteAggregation\\RouteTable.xlsx";
                ////string connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.12.0; data source={0}; Extended Properties=\"Excel 12.0;HDR=No;IMEX=1\";", fullPath);
                //string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullPath + ";Extended Properties=Excel 12.0;";

                //OleDbCommand selectCommand = new OleDbCommand();
                //OleDbConnection connection = new OleDbConnection();
                //OleDbDataAdapter adapter = new OleDbDataAdapter();
                //connection.ConnectionString = connectionString;

                //if (connection.State != ConnectionState.Open)
                //    connection.Open();

                //DataTable dtSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                ////List<string> SheetsName = GetSheetsName(dtSchema);
                ////for (int i = 0; i < SheetsName.Count; i++)
                ////{
                //selectCommand.CommandText = "SELECT * FROM [RouteTable$]";
                //selectCommand.Connection = connection;
                //adapter.SelectCommand = selectCommand;
                //DataSet ds = new DataSet();

                //adapter.Fill(ds);
                using (StreamReader sr = new StreamReader("WholeTable.csv"))
                {
                    int i=0;
                    while(sr.Peek()>=0)
                    {
                        string[] route = sr.ReadLine().Split(',');
                        Route row = new Route();
                        row.IPAddress = route[0];
                        row.Path = route[2];
                        row.Segment = Convert.ToInt16(route[1]);
                        routeTable.Add(row);
                        i++;
                        if (i % 200 == 0)
                        {
                            Console.WriteLine(i + "records read");
                        }
                    }
                }
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{
                //    if (dr[0].ToString() == "1.2.4.0")
                //        Console.ReadKey();
                //    Route row = new Route();
                //    row.IPAddress = dr[0].ToString();
                //    row.Path = dr[2].ToString();
                //    row.Segment = Convert.ToInt16(dr[1]);
                    
                //    routeTable.Add(row);
                //}
                return routeTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public long ipAddresstoInt(string ipAddress)
        {
            long integerIp = 0;
            //string checkedIp = "";
            string[] parts = ipAddress.Split('.');
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
        public void GetRange(long address, short segment, out long start, out long end)
        {
            start = 0;
            end = 0;
            string s = new String('1', segment);
            s += new String('0', 32 - segment);

            //Byte[] bytedata;
            
            long mask = Convert.ToInt64(s, 2);
            start = address & mask;
            mask = ~mask;
            s = new String('0', segment);
            s += new String('1', 32 - segment);

            //Byte[] bytedata;

            mask = Convert.ToInt64(s, 2);
            end = address ^ (mask);
        }
        public List<RangeRoute> MakeNewList(List<Route> routelist)
        {
            List<RangeRoute> rangelist = new List<RangeRoute>();
            int i = 0;
            foreach (Route r in routelist)
            {
                //if (r.IPAddress == "1.2.4.0")
                //    Console.ReadKey();
                long start;
                long end;
                long integerIp = r.ipAddresstoInt();
                GetRange(integerIp, r.Segment, out start, out end);
                int index = rangelist.FindIndex(x => ((x.upper+1 == start)||(x.lower-1==end)) && (x.path == r.Path));
                if (index == -1)
                {
                    RangeRoute range = new RangeRoute();
                    range.upper = end;
                    range.lower = start;
                    range.highSegment = r.Segment;
                    range.integerIp = integerIp;
                    range.path = r.Path;
                    rangelist.Add(range);
                }
                else
                {
                    if(rangelist[index].upper+1==start)
                        rangelist[index].upper = end;
                    else if(rangelist[index].lower-1==end)
                        rangelist[index].lower = start;
                    if (r.Segment > rangelist[index].highSegment)
                        rangelist[index].highSegment = r.Segment;
                    
                }
                i++;
                if (i % 100 == 0)
                    Console.WriteLine(i + "Records processed");
            }
            var result = rangelist.GroupBy(x => new { x.upper, x.lower, x.path, x.integerIp })
                .Select(x => new RangeRoute
                {
                    upper = x.Key.upper,
                    lower = x.Key.lower,
                    path = x.Key.path,
                    integerIp = x.Key.integerIp,
                    highSegment = x.Max(x1 => x1.highSegment)
                });
            return result.ToList();
        }

        internal void WriteTofile(List<RangeRoute> rangeList)
        {
            //rangeList.Distinct
            using (StreamWriter sw = File.CreateText("list.csv"))
            {
                sw.WriteLine("IPValue,Lower,Upper,Path,HighSegment");
                foreach(RangeRoute r in rangeList)
                {
                    sw.WriteLine(r.integerIp.ToString() + "," + r.lower.ToString() + "," + r.upper.ToString() + "," + r.path.ToString() + "," + r.highSegment.ToString());
                    
                }
            }
        }
    }
}
