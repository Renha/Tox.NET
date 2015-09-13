using System;
using System.Runtime.InteropServices;

namespace Tox.Sodium
{
    internal static class NativeMethods
    {
        const string dllName = "libsodium";

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_box_afternm")]
        public static extern int CryptoBoxAfterNm(byte[] c, byte[] m, long mlen, byte[] n, byte[] k);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_box_open_afternm")]
        public static extern int CryptoBoxOpenAfterNm(byte[] m, byte[] c, long clen, byte[] n, byte[] k);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_box_beforenm")]
        public static extern int CryptoBoxBeforeNm(byte[] k, byte[] pk, byte[] sk);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_box_keypair")]
        public static extern void CryptoBoxKeypair(byte[] publicKey, byte[] secretKey);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "randombytes_buf")]
        public static extern void RandomBytesBuf(byte[] buffer, int size);
    }
}