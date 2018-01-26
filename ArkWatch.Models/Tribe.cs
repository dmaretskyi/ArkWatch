using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArkWatch.Models
{
    [Serializable]
    public sealed class Tribe
    {
        public string Name { get; set; }

        public List<string> Members { get; }

        public Tribe(string name, List<string> members)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Members = new List<string>(members ?? throw new ArgumentNullException(nameof(members)));
        }
    }
}
