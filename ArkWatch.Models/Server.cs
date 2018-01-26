using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArkWatch.Models
{
    [Serializable]
    public sealed class Server
    {
        public const int DefaultArkServerPort = 27015;

        public string Address { get; set; }

        public string Name { get; set; }

        public IPEndPoint GetIpEndPoint()
        {
            var parts = Address.Split(':');
            if (parts.Length == 1)
            {
                return new IPEndPoint(IPAddress.Parse(parts[0]), DefaultArkServerPort);
            }
            else
            {
                return new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
            }
        }

        public Server(string address, string name)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        private bool Equals(Server other)
        {
            return string.Equals(Address, other.Address) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Server && Equals((Server) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Address.GetHashCode() * 397) ^ Name.GetHashCode();
            }
        }

        public static bool operator ==(Server left, Server right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Server left, Server right)
        {
            return !Equals(left, right);
        }
    }
}
