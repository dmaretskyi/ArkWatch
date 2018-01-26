using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GameServerInfo;

namespace ArkWatch.Sandbox.QueryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var info = ServerQuery.ServerQuery.Query(new IPEndPoint(IPAddress.Parse("217.66.97.19"), 27015));

            Console.WriteLine($"Server name: {info.Name}");

            Console.WriteLine($"{info.Players.Count()} players:");
            foreach (var player in info.Players)
            {
                Console.WriteLine(player.Name);
            }

            Console.ReadKey();
        }
    }
}
