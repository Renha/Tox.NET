using System;
using System.Net;

namespace Tox.Network
{
    internal class PacketHandler
    {
        private Action<IPEndPoint, byte[]> _action;

        public PacketHandler(Action<IPEndPoint, byte[]> action)
        {
            _action = action;
        }

        public void Invoke(IPEndPoint endpoint, byte[] data)
        {
            _action.Invoke(endpoint, data);
        }
    }
}
