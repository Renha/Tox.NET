using System;
using Tox.Sodium;
using Tox.Network.Packets;

namespace Tox.Network
{
    internal abstract class PingPacket : IToxPacket
    {
        public abstract PacketID ID { get; }
        public ulong PingID { get; private set; }

        private byte[] _publicKey;

        protected PingPacket(byte[] publicKey, ulong pingID)
        {
            _publicKey = publicKey;
            PingID = pingID;
        }

        protected PingPacket(byte[] data, byte[] sharedKey)
        {
            //0 - packet id
            //1 - public key
            //2 - nonce
            //3 - encrypted
            byte[][] pieces = Tools.SplitBytes(data, 1, 32, 24, data.Length - (1 + 32 + 24));
            byte[] plain = CryptoBox.DecryptSymmetric(sharedKey, pieces[2], pieces[3], pieces[3].Length);

            //0 - packet id
            //1 - ping id
            byte[][] plainPieces = Tools.SplitBytes(plain, 1, sizeof(ulong));
            PingID = BitConverter.ToUInt64(plainPieces[1], 0);
        }

        public byte[] Pack(byte[] sharedKey = null)
        {
            if (sharedKey == null)
                throw new Exception("No shared key specified");

            byte[] pingIDBytes = BitConverter.GetBytes(PingID);
            byte[] nonce = CryptoRandom.NextNonce();

            byte[] pingPlain = Tools.ConcatBytes(new byte[] { (byte)ID }, pingIDBytes);
            byte[] encrypted = CryptoBox.EncryptSymmetric(sharedKey, nonce, pingPlain);

            return Tools.ConcatBytes(new byte[] { (byte)ID }, _publicKey, nonce, encrypted);
        }
    }
}
