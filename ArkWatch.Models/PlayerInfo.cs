using System;

namespace ArkWatch.Models
{
    public class PlayerInfo
    {
        public string Name { get; }

        public PlayerInfo(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}