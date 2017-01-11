using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading;

namespace New_Chet
{
    class Program
    {

        const string _id = "";
        const string _password = "";
        const string _servername = "";
        const string _catalog = "";
        const string _query = "Select t1.playerId, t1.uniqueid, t2.lastAddress from hlstats_playeruniqueids as t1 INNER JOIN hlstats_players as t2 where t1.playerId=t2.playerId and t1.uniqueId NOT LIKE 'BOT%' ORDER BY t1.playerId ASC ";
        static void Main(string[] args)
        {
            Program ps = new Program();
            ps.start();
        }

        private void start()
        {
            DataTable table = new DataTable();
            string constring = String.Format("server={0};userid={1};password={2};database={3}", _servername, _id, _password, _catalog);
            MySqlConnection conn = new MySqlConnection(constring);
            conn.Open();
            MySqlDataAdapter AS = new MySqlDataAdapter(_query, conn);

            AS.Fill(table);
            conn.Close();

            List<ipdata> list = new List<ipdata>();

            list = (from DataRow dr in table.Rows

                    select new ipdata(dr["lastAddress"].ToString())
                  //  select new ipdata()
                    {
                        playerId = Convert.ToInt32(dr["playerId"].ToString()),
                        uniqueid = steam64id(Convert.ToString(dr["uniqueId"])),
                        IP = dr["lastAddress"].ToString(),

                    }).ToList();

            foreach( var item in list)
            {
                int hello = inserData(item);
                if(hello==1)
                {
                    Console.WriteLine("Insert success for " + item.playerId.ToString());
                }
                else
                    Console.WriteLine("Insert failed for " + item.playerId.ToString());
            }


          //  string l = "10";
            
        }

        public ipinfo JSONReady(string ip)
        {
            ipinfo ew;
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                string url = String.Format("http://extreme-ip-lookup.com/json/{0}", ip);
                Uri URL = new Uri(url);
                //
                var json = wc.DownloadString(URL);
                JObject jo = JObject.Parse(json);

              
                ew.isp = (string)jo["isp"];
                ew.org = (string)jo["org"];
               

            }
            return ew;
            // return string;
        }

       public struct ipinfo
        {
            public string isp;
            public string org;
        }
        int count = 0;
        public string steam64id(string idt)
        {
           
            string[] _temp = idt.Split(':');

            string _id64 = Convert.ToString((Convert.ToInt32(_temp[0]) + 76561197960265728) + (Convert.ToInt32(_temp[1]) * 2));

            count++;
            Console.WriteLine("ID No: " + count);
            Console.WriteLine("Time: " + DateTime.Now.ToString());

            return _id64;
        }

       public int inserData(ipdata info)
        {
            
                 string constring = String.Format("server={0};userid={1};password={2};database={3}", _servername, _id, _password, _catalog);
            MySqlConnection c = new MySqlConnection(constring);
            MySqlCommand cmd = new MySqlCommand(@"INSERT INTO `ipinfo` (`playerId`, `uniqueid`, `IP`, `ISP`, `ORG`) VALUES (@pid, @pstid, @pip, @pisp,@porg)");
            MySqlParameter p1 = new MySqlParameter("@pid", MySqlDbType.Int32);
            p1.Value = info.playerId;

            MySqlParameter p2 = new MySqlParameter("@pstid", MySqlDbType.VarChar);
            p2.Value = info.uniqueid;
                 MySqlParameter p3 = new MySqlParameter("@pip", MySqlDbType.VarChar);
            p3.Value = info.IP;
                 MySqlParameter p4 = new MySqlParameter("@pisp", MySqlDbType.VarChar);
            p4.Value = info.ISP;
                 MySqlParameter p5 = new MySqlParameter("@porg", MySqlDbType.VarChar);
            p5.Value = info.ORG;

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);

            c.Open();
            cmd.Connection = c;
            var test = cmd.ExecuteNonQuery();
            c.Close();
            Thread.Sleep(30);

            return test;
        }

    }
    class ipdata
    {
        public ipdata()
        {

        }
        public ipdata(string ip)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                string url = String.Format("http://extreme-ip-lookup.com/json/{0}", ip);
                Uri URL = new Uri(url);
                //
                var json = wc.DownloadString(URL);
                Thread.Sleep(300);
                JObject jo = JObject.Parse(json);
                ISP = (string)jo["isp"];
                ORG = (string)jo["org"];


            }
        }
        public int playerId { get; set; }
        public string uniqueid { get; set; }
        public string IP { get; set; }

        
        public string ISP { get; set; }
        public string ORG { get; set; }


    }
}
