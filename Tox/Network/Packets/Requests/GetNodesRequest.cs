using System;
using Tox.Network.Packets;
using Tox.Sodium;

namespace Tox.Network.Packets
{
    internal class GetNodesRequest : IToxPacket
    {
        public PacketID ID { get { return PacketID.GetNodes; } }
        public byte[] PublicKey { get; private set; }
        public ulong PingID { get; private set; }

        private byte[] _selfPublicKey;

        public byte[] Pack(byte[] sharedKey = null)
        {
            if (sharedKey == null)
                throw new Exception("No shared key specified");

            byte[] nonce = CryptoRandom.NextNonce();
            byte[] plain = Tools.ConcatBytes(PublicKey, BitConverter.GetBytes(PingID));

            byte[] encrypt = CryptoBox.EncryptSymmetric(sharedKey, nonce, plain);
            return Tools.ConcatBytes((byte)ID, _selfPublicKey, nonce, encrypt);
        }

        public GetNodesRequest(byte[] selfPublicKey, byte[] queryPublicKey)
        {
            _selfPublicKey = selfPublicKey;
            PublicKey = queryPublicKey;
            PingID = CryptoRandom.NextUInt64();
        }
    }
}