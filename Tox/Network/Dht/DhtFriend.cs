using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;

namespace Tox.Network
{
    internal class DhtFriend
    {
        public IPEndPoint EndPoint { get; private set; }
        public NodeID PublicKey { get; private set; }
        public byte[] SharedKey { get; private set; }
        public DateTime LastPingTime { get; private set; }

        private List<Ping> _pings = new List<Ping>();

        public DhtFriend(IPEndPoint endpoint, byte[] publicKey, byte[] sharedKey)
        {
            EndPoint = endpoint;
            PublicKey = publicKey;
            SharedKey = sharedKey;
        }

        public void AddPing(ulong pingID)
        {
            _pings.Add(new Ping(DateTime.Now, pingID));
        }

        public bool ContainsPing(ulong pingID)
        {
            return _pings.Any(p => p.ID == pingID);
        }

        public void RemovePing(ulong pingID)
        {
            if (_pings.RemoveAll(p => p.ID == pingID) > 0)
                LastPingTime = DateTime.Now;
        }
    }
}
