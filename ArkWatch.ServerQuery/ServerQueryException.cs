using System;
using System.Runtime.Serialization;

namespace ArkWatch.ServerQuery
{
    [Serializable]
    public class ServerQueryException : Exception
    {
        public ServerQueryException()
        {
        }

        public ServerQueryException(string message) : base(message)
        {
        }

        public ServerQueryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ServerQueryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}