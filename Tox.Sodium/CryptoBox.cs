using System;

namespace Tox.Sodium
{
    public class CryptoBox
    {
        public const int PublicKeySize = 32;
        public const int SecretKeySize = 32;
        public const int NonceSize = 24;
        public const int MacSize = 16;

        public static byte[] DecryptSymmetric(byte[] sharedKey, byte[] nonce, byte[] encryptedData, int length)
        {
            byte[] plain = new byte[length - 16];
            byte[] temp_plain = new byte[length + 32];
            byte[] temp_encrypted = new byte[length + 16];

            Array.Copy(encryptedData, 0, temp_encrypted, 16, length);

            OpenAfterNm(temp_plain, temp_encrypted, length + 16, nonce, sharedKey);
            Array.Copy(temp_plain, 32, plain, 0, length - 16);

            return plain;
        }

        public static byte[] EncryptSymmetric(byte[] sharedKey, byte[] nonce, byte[] plainData)
        {
            byte[] encrypt = new byte[plainData.Length + 16];
            byte[] temp_plain = new byte[plainData.Length + 32];
            byte[] temp_encrypted = new byte[plainData.Length + 16 + 16];
            Array.Copy(plainData, 0, temp_plain, 32, plainData.Length);

            AfterNm(temp_encrypted, temp_plain, plainData.Length + 32, nonce, sharedKey);
            Array.Copy(temp_encrypted, 16, encrypt, 0, plainData.Length + 16);

            return encrypt;
        }

        public static KeyPair GenerateKeyPair()
        {
            byte[] publicKey = new byte[PublicKeySize];
            byte[] secretKey = new byte[SecretKeySize];

            NativeMethods.CryptoBoxKeypair(publicKey, secretKey);

            return new KeyPair(publicKey, secretKey);
        }

        public static byte[] BeforeNm(byte[] publicKey, byte[] secretKey)
        {
            byte[] sharedKey = new byte[PublicKeySize];
            int ret = NativeMethods.CryptoBoxBeforeNm(sharedKey, publicKey, secretKey);

            if (ret != 0)
                throw new Exception("Failed to generate shared key");

            return sharedKey;
        }

        private static void AfterNm(byte[] c, byte[] m, long length, byte[] n, byte[] k)
        {
            if (NativeMethods.CryptoBoxAfterNm(c, m, length, n, k) != 0)
                throw new Exception("Failed");
        }

        private static void OpenAfterNm(byte[] m, byte[] c, long clen, byte[] n, byte[] k)
        {
            if (NativeMethods.CryptoBoxOpenAfterNm(m, c, clen, n, k) != 0)
                throw new Exception("Failed");
        }
    }
}
