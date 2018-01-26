using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArkWatch.Models;

namespace Storage
{
    [Serializable]
    public class StorageData
    {
        public IList<Server> Servers { get; } 

        public IList<Player> Players { get; }

        public IList<Tribe> Tribes { get; }

        public StorageData(IList<Server> servers, IList<Player> players, IList<Tribe> tribes)
        {
            Servers = new List<Server>(servers ?? throw new ArgumentNullException(nameof(servers)));
            Players = new List<Player>(players ?? throw new ArgumentNullException(nameof(players)));
            Tribes = new List<Tribe>(tribes ?? throw new ArgumentNullException(nameof(tribes)));
        }
    }
}
