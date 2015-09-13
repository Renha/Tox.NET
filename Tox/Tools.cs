using System;
using System.Text;
using System.Linq;

namespace Tox
{
    internal static class Tools
    {
        public static string HexBinToString(byte[] b)
        {
            StringBuilder sb = new StringBuilder(2 * b.Length);

            for (int i = 0; i < b.Length; i++)
                sb.AppendFormat("{0:X2}", b[i]);

            return sb.ToString();
        }

        public static byte[] StringToHexBin(string s)
        {
            byte[] bin = new byte[s.Length / 2];

            for (int i = 0; i < bin.Length; i++)
                bin[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);

            return bin;
        }

        public static byte[] ConcatBytes(params byte[][] arrays)
        {
            byte[] result = new byte[arrays.Sum(a => a.Length)];
            int pos = 0;

            foreach (byte[] array in arrays)
            {
                Array.Copy(array, 0, result, pos, array.Length);
                pos += array.Length;
            }

            return result;
        }

        public static byte[][] SplitBytes(byte[] bytes, params int[] lengths)
        {
            byte[][] pieces = new byte[lengths.Length][];
            int position = 0;

            for (int i = 0; i < pieces.Length; i++)
            {
                int length = lengths[i];

                pieces[i] = new byte[length];
                Array.Copy(bytes, position, pieces[i], 0, length);

                position += length;
            }

            return pieces;
        }
    }
}
