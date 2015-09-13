using System;
using System.Net;
using System.Text;

namespace Tox.Network.Packets
{
    internal class BootstrapInfoResponse : IToxPacket
    {
        public PacketID ID { get { return PacketID.BootstrapInfo; } }

        public int Version { get; private set; }
        public string Motd { get; private set; }

        public BootstrapInfoResponse(byte[] data)
        {
            Version = IPAddress.HostToNetworkOrder((int)BitConverter.ToUInt32(data, 1));
            Motd = Encoding.UTF8.GetString(data, 1 + sizeof(uint), data.Length - (1 + sizeof(uint)));
        }

        public byte[] Pack(byte[] sharedKey = null)
        {
            return null;
        }
    }
}
