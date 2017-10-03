// 
// Copyright (c) 2017, Norbert Wagner (nw@bytefeld.com)
//
using System;
using System.Collections;
using System.Text;

namespace Bytefeld.Epc
{
    /// <summary>
    /// Class EpcEncoder
    /// </summary>
    internal static class EpcEncoder
    {
        /// <summary>
        /// Class Partition
        /// </summary>
        internal class Partition
        {
            /// <summary>
            /// Gets or sets the bits1.
            /// </summary>
            /// <value>The bits1.</value>
            public int Bits1 { get; set; }

            /// <summary>
            /// Gets or sets the digits1.
            /// </summary>
            /// <value>The digits1.</value>
            public int Digits1 { get; set; }

            /// <summary>
            /// Gets or sets the bits2.
            /// </summary>
            /// <value>The bits2.</value>
            public int Bits2 { get; set; }

            /// <summary>
            /// Gets or sets the digits2.
            /// </summary>
            /// <value>The digits2.</value>
            public int Digits2 { get; set; }
        }


        /// <summary>
        /// Decodes the hex char.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>System.Byte.</returns>
        /// <exception cref="System.FormatException"></exception>
        private static uint DecodeHexChar(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return (uint)(c - '0');
            }
            else if (c >= 'A' && c <= 'F')
            {
                return (uint)(10 + c - 'A');
            }
            else if (c >= 'a' && c <= 'f')
            {
                return (uint)(10 + c - 'a');
            }
            else
                throw new FormatException();
        }

        /// <summary>
        /// Returns the hex character that represents the specified nibble
        /// </summary>
        /// <param name="nibble"></param>
        /// <returns></returns>
        private static char EncodeHexChar(byte nibble)
        {
            if (nibble <= 9)
            {
                return (char) ('0' + nibble);
            }
            else if (nibble >= 10 && nibble <= 15)
            {
                return (char)('A' + nibble - 10);
            }
            else
                throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Converts to bit array.
        /// </summary>
        /// <param name="epcCode">The epc code.</param>
        /// <returns>BitArray.</returns>
        public static BitArray BinaryStringToBitArray(string epcCode)
        {
            BitArray bits = new BitArray(epcCode.Length*4);
            for (int i = 0; i < epcCode.Length; i++)
            {
                var b = DecodeHexChar(epcCode[i]);
                for (int bit = 0; bit < 4; bit++)
                {
                    bits[i * 4 + bit] = (b & (0x08 >> bit)) != 0;
                }
            }
            return bits;
        }

        /// <summary>
        /// Converts the specified bit array into a binary EPC string representation
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static string BitArrayToBinaryString(BitArray bits)
        {
            var bitCount = bits.Length + (16 - bits.Length % 16) % 16;
            var charCount = bitCount / 4;

            StringBuilder sb = new StringBuilder(charCount);
            for (int i = 0; i < charCount; i++)
            {
                int start = i * 4;
                int len = Math.Min(4, bits.Length - start);
                if (len > 0)
                    sb.Append(EncodeHexChar(bits.DecodeByte(start, len)));
                else
                    sb.Append('0');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parses the partition table.
        /// </summary>
        /// <param name="bits">The bit array.</param>
        /// <param name="partitions">The tag type specific partition table.</param>
        /// <param name="firstBit">The first bit where the partion starts.</param>
        /// <param name="partition">The resulting partition.</param>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <exception cref="System.FormatException"></exception>
        public static void DecodePartition(this BitArray bits, Partition[] partitions, int firstBit, out byte partition, out string val1, out string val2)
        {
            partition = (byte)EpcEncoder.DecodeUInt32(bits, firstBit, 3);
            if (partitions.Length < partition || partitions[partition] == null)
                throw new FormatException(string.Format("Partition {0} not defined", partition));

            var partitionDef = partitions[partition];

            int bits1 = partitionDef.Bits1;
            int bits2 = partitionDef.Bits2;
            ulong num1 = EpcEncoder.ReadUInt64(bits, firstBit + 3, bits1);
            ulong num2 = EpcEncoder.ReadUInt64(bits, firstBit + 3 + bits1, bits2);

            val1 = num1.ToString().PadLeft(partitionDef.Digits1, '0');
            val2 = num2.ToString().PadLeft(partitionDef.Digits2, '0');
        }

        public static void EncodePartition(this BitArray bits, Partition[] partitions, int firstBit, byte partition, string val1, string val2)
        {
            if (partitions.Length < partition || partitions[partition] == null)
                throw new FormatException(string.Format("Partition {0} not defined", partition));

            var partitionDef = partitions[partition];
            int bits1 = partitionDef.Bits1;
            int bits2 = partitionDef.Bits2;

            bits.Encode(partition, firstBit, 3);

            val1 = val1.TrimStart('0');
            val2 = val2.TrimStart('0');

            if (val1.Length > partitionDef.Digits1)
                throw new FormatException("Value1 too long for specified partition");
            ulong num1 = UInt64.Parse(val1);

            if (val2.Length > partitionDef.Digits2)
                throw new FormatException("Value2 too long for specified partition");
            ulong num2 = UInt64.Parse(val2);

            bits.Encode(num1, firstBit + 3, bits1);
            bits.Encode(num2, firstBit + 3 + bits1, bits2);
        }

        /// <summary>
        /// Reads a string from the bit array.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="firstBit">The first bit.</param>
        /// <param name="bitCount">The bit count.</param>
        /// <returns>System.UInt32.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">length;Length must be in [0...31]</exception>
        public static string DecodeString(this BitArray bits, int firstBit, int bitCount)
        {
            Assert.MultipleOf("BitCount", bitCount, 7);
            if (firstBit < 0 || firstBit + bitCount > bits.Length)
                throw new ArgumentOutOfRangeException("firstBit", "firstBit must be in [0...bits.Length-length]");

            int bitPos = firstBit;
            int lastBit = firstBit + bitCount - 1;

            StringBuilder sb = new StringBuilder(40);
            while( bitPos < lastBit)
            {
                char c = (char)DecodeUInt32(bits, bitPos, 7);
                if (c == 0)
                    break;

                if ( c == '%' ) 
                {
                    var ascii 
                        = (DecodeHexChar( (char)DecodeUInt32(bits, bitPos+7, 7)) << 4)
                        + DecodeHexChar( (char)DecodeUInt32(bits, bitPos+14, 7)) ;
                    c = (char)ascii;
                    bitPos += 21;
                }
                else
                {
                    bitPos += 7;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        public static void EncodeString(this BitArray bits, string value, int firstBit, int bitCount)
        {
            Assert.MultipleOf("BitCount", bitCount, 7);
            if (firstBit < 0 || firstBit + bitCount > bits.Length)
                throw new ArgumentOutOfRangeException("firstBit", "firstBit must be in [0...bits.Length-length]");

            int bitPos = firstBit;
            int lastBit = firstBit + bitCount - 1;
            int charCount = bitCount / 7;

            int valueIdx = 0;
            for (int bitIdx = firstBit; bitIdx < lastBit; bitIdx += 7)
            {
                if (valueIdx >= value.Length)
                    break;

                uint ascii = value[valueIdx];
                if (ascii == '%' && valueIdx + 2 < value.Length)
                {
                    // decode %xx escaped character
                    ascii = (DecodeHexChar(value[valueIdx + 1]) << 4) + DecodeHexChar(value[valueIdx + 2]);
                    valueIdx += 3;
                }
                else
                {
                    // decode regular character
                    valueIdx += 1;
                }
                bits.Encode(ascii, bitIdx, 7);
            }
        }

        public static byte DecodeByte(this BitArray bits, int firstBit, int bitCount)
        {
            Assert.InRange("BitCount", bitCount, 1, 8);

            if (firstBit < 0 || firstBit + bitCount > bits.Length)
                throw new ArgumentOutOfRangeException("firstBit", "firstBit must be in [0...bits.Length-length]");

            byte result = 0;
            for (int i = 0; i < bitCount; i++)
            {
                if (bits[firstBit + bitCount - 1 - i])
                    result |= (byte)(1u << i);
            }

            return result;
        }

        /// <summary>
        /// Reads an unsigned int32 from the bit array
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="firstBit">The first bit.</param>
        /// <param name="bitCount">The number of bits to read.</param>
        /// <returns>System.UInt32.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">bitCount must be in [0...31]</exception>
        public static uint DecodeUInt32(this BitArray bits, int firstBit, int bitCount)
        {
            Assert.InRange("BitCount", bitCount, 1, 32);

            if (firstBit < 0 || firstBit + bitCount > bits.Length)
                throw new ArgumentOutOfRangeException("firstBit", "firstBit must be in [0...bits.Length-length]");

            uint result = 0;
            for (int i = 0; i < bitCount; i++)
            {
                if (bits[firstBit + bitCount - 1 - i])
                    result |= (1u << i);
            }

            return result;
        }

        public static void Encode(this BitArray bits, byte value, int firstBit, int bitCount)
        {
            Assert.InRange("BitCount", bitCount, 1, 8);

            Encode(bits, (ulong) value, firstBit, bitCount);
        }

        public static void Encode(this BitArray bits, uint value, int firstBit, int bitCount)
        {
            Assert.InRange("BitCount", bitCount, 1, 32);

            Encode(bits, (ulong)value, firstBit, bitCount);
        }

        public static void Encode(this BitArray bits, ulong value, int firstBit, int bitCount)
        {
            Assert.InRange("BitCount", bitCount, 1, 64);

            if (firstBit < 0 || firstBit + bitCount > bits.Length)
                throw new ArgumentOutOfRangeException("firstBit", "firstBit must be in [0...bits.Length-length]");

            for (int i = 0; i < bitCount; i++)
            {
                var bit = ((value & (1ul << i)) != 0);
                bits[firstBit + bitCount - 1 - i] = bit;
            }
        }

        /// <summary>
        /// Gets the unsigned int64.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="firstBit">The first bit.</param>
        /// <param name="bitCount">The number of bits to read.</param>
        /// <returns>System.UInt64.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">bitCount must be in [0...63]</exception>
        public static ulong ReadUInt64(BitArray bits, int firstBit, int bitCount)
        {
            Assert.InRange("BitCount", bitCount, 1, 64);
            if (firstBit < 0 || firstBit + bitCount > bits.Length)
                throw new ArgumentOutOfRangeException("firstBit", "firstBit must be in [0...bits.Length-length]");

            ulong result = 0;
            for (int i = 0; i < bitCount; i++)
            {
                if (bits[firstBit + bitCount - 1 - i])
                    result |= (1ul << i);
            }

            return result;
        }
    }
}
