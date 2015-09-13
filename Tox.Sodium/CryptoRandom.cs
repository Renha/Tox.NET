using System;

namespace Tox.Sodium
{
    public static class CryptoRandom
    {
        public static ulong NextUInt64()
        {
            byte[] bytes = GetRandomBytes(sizeof(ulong));
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static uint NextUInt32()
        {
            byte[] bytes = GetRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static byte[] NextNonce()
        {
            return GetRandomBytes(CryptoBox.NonceSize);
        }

        private static byte[] GetRandomBytes(int count)
        {
            byte[] buffer = new byte[count];
            NativeMethods.RandomBytesBuf(buffer, count);

            return buffer;
        }
    }
}
