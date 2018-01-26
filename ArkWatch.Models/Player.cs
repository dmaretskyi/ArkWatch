using System;

namespace ArkWatch.Models
{
    [Serializable]
    public class Player
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        public Player(string name, string comment)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }
    }
}