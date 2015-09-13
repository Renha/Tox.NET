using System;

namespace Tox.Network.Packets
{
    internal class PingResponse : PingPacket
    {
        public override PacketID ID { get { return PacketID.PingResponse; } }

        public PingResponse(byte[] publicKey, ulong pingID)
            : base(publicKey, pingID) { }

        public PingResponse(byte[] data, byte[] sharedKey)
            : base(data, sharedKey) { }
    }
}