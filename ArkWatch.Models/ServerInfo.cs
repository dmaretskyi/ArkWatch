using System;
using System.Collections.Generic;

namespace ArkWatch.Models
{
    public class ServerInfo
    {
        public string Name { get; }

        public IReadOnlyList<PlayerInfo> Players { get; }

        public ServerInfo(string name, IEnumerable<PlayerInfo> players)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Players = new List<PlayerInfo>(players ?? throw new ArgumentNullException(nameof(players)));
        }
    }
}