using System;
using System.Collections.Generic;
using Tox.Network.Packets;
using Tox.Sodium;
using System.Net;

namespace Tox.Network
{
    internal class SendNodesResponse : IToxPacket
    {
        public PacketID ID { get { return PacketID.SendNodesIpv6; } }
        public List<Tuple<IPEndPoint, NodeID>> Nodes { get; private set; }
        public ulong PingID { get; private set; }

        public SendNodesResponse(byte[] data, byte[] sharedKey)
        {
            Nodes = new List<Tuple<IPEndPoint, NodeID>>();

            //0 - packet id
            //1 - public key
            //2 - nonce
            //3 - encrypted data
            byte[][] pieces = Tools.SplitBytes(data, 1, 32, 24, data.Length - (1 + 32 + 24));
            byte[] plain = CryptoBox.DecryptSymmetric(sharedKey, pieces[2], pieces[3], pieces[3].Length);

            int nodeCount = plain[0];
            int processed = 1;

            for (int i = 0; i < nodeCount; i++)
            {
                bool ipv6;

                switch (plain[processed])
                {
                    case 2: //TOX_AF_INET
                    case 160: //TOX_TCP_INET
                        ipv6 = false;
                        break;
                    case 10: //TOX_AF_INET6
                    case 138: //TOX_TCP_INET6
                        ipv6 = true;
                        break;
                    default: //received odd data
                        return;

                }

                processed++;

                int size = (ipv6 ? 16 : 4) + sizeof(ushort) + 32;
                byte[] nodeBytes = new byte[size];
                Array.Copy(plain, processed, nodeBytes, 0, size);

                //0 - ip
                //1 - port
                //2 - public key
                byte[][] nodePieces = Tools.SplitBytes(nodeBytes, ipv6 ? 16 : 4, sizeof(ushort), 32);

                IPAddress ip = new IPAddress(nodePieces[0]);
                int port = (ushort)IPAddress.HostToNetworkOrder((short)BitConverter.ToUInt16(nodePieces[1], 0));

                Nodes.Add(Tuple.Create<IPEndPoint, NodeID>(new IPEndPoint(ip, port), nodePieces[2]));
                processed += size;
            }

            byte[] pingID = new byte[sizeof(ulong)];
            Array.Copy(plain, processed, pingID, 0, pingID.Length);
            PingID = BitConverter.ToUInt64(pingID, 0);
        }

        public byte[] Pack(byte[] sharedKey = null)
        {
            throw new NotImplementedException();
        }
    }
}
