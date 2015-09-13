using System;

namespace Tox.Network.Packets
{
    internal interface IToxPacket
    {
        PacketID ID { get; }
        byte[] Pack(byte[] sharedKey = null);
    }
}