using System;
using Tox.Network;
using Tox.Network.Packets;

namespace Tox.Network.Packets
{
    internal class PingRequest : PingPacket
    {
        public override PacketID ID { get { return PacketID.PingRequest; } }

        public PingRequest(byte[] publicKey, ulong pingID)
            : base(publicKey, pingID) { }

        public PingRequest(byte[] data, byte[] sharedKey)
            : base(data, sharedKey) { }
    }
}
