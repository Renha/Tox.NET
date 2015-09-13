using System;

namespace Tox.Sodium
{
    public class KeyPair
    {
        public byte[] PublicKey { get; private set; }
        public byte[] SecretKey { get; private set; }

        public KeyPair(byte[] publicKey, byte[] secretKey)
        {
            PublicKey = publicKey;
            SecretKey = secretKey;
        }
    }
}
