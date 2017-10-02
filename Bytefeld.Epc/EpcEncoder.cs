// 
// Copyright (c) 2017, Norbert Wagner (nw@bytefeld.com)
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private static int DecodeHexChar(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return (int)c - (int)'0';
            }
            else if (c >= 'A' && c <= 'F')
            {
                return 10 + (int)c - (int)'A';
            }
            else if (c >= 'a' && c <= 'f')
            {
                return 10 + (int)c - (int)'a';
            }
            else
                throw new FormatException();
        }

        /// <summary>
        /// Converts to bit array.
        /// </summary>
        /// <param name="epcCode">The epc code.</param>
        /// <returns>BitArray.</returns>
        public static BitArray ConvertToBitArray(string epcCode)
        {
            BitArray bits = new BitArray(epcCode.Length*4);
            for (int i = 0; i < epcCode.Length; i++)
            {
                int b = DecodeHexChar(epcCode[i]);
                for (int bit = 0; bit < 4; bit++)
                {
                    bits[i * 4 + bit] = (b & (0x08 >> bit)) != 0;
                }
            }
            return bits;
        }

        /// <summary>
        /// Parses the partition table.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="firstBit">The first bit.</param>
        /// <param name="partitions">The partitions.</param>
        /// <param name="partition">The partition.</param>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <exception cref="System.FormatException"></exception>
        public static void ParsePartitionTable(BitArray bits, int firstBit, Partition[] partitions, out byte partition, out string val1, out string val2)
        {
            partition = (byte)EpcEncoder.GetUnsignedInt32(bits, firstBit, 3);
            if (partitions.Length < partition || partitions[partition] == null)
                throw new FormatException(string.Format("Partition {0} not defined", partition));

            var partitionDef = partitions[partition];

            int bits1 = partitionDef.Bits1;
            int bits2 = partitionDef.Bits2;
            ulong num1 = EpcEncoder.GetUnsignedInt64(bits, firstBit + 3, bits1);
            ulong num2 = EpcEncoder.GetUnsignedInt64(bits, firstBit + 3 + bits1, bits2);

            val1 = num1.ToString().PadLeft(partitionDef.Digits1, '0');
            val2 = num2.ToString().PadLeft(partitionDef.Digits2, '0');
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="firstBit">The first bit.</param>
        /// <param name="bitCount">The bit count.</param>
        /// <returns>System.UInt32.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">length;Length must be in [0...31]</exception>
        public static string GetString(BitArray bits, int firstBit, int bitCount)
        {
            Assert.MultipleOf("BitCount", bitCount, 7);
            if (firstBit < 0 || firstBit + bitCount > bits.Length)
                throw new ArgumentOutOfRangeException("firstBit", "firstBit must be in [0...bits.Length-length]");

            int bitPos = firstBit;
            int lastBit = firstBit + bitCount - 1;

            StringBuilder sb = new StringBuilder(40);
            while( bitPos < lastBit)
            {
                char c = (char)GetUnsignedInt32(bits, bitPos, 7);
                if (c == 0)
                    break;

                if ( c == '%' ) 
                {
                    int ascii 
                        = (DecodeHexChar( (char)GetUnsignedInt32(bits, bitPos+7, 7)) << 4)
                        + DecodeHexChar( (char)GetUnsignedInt32(bits, bitPos+14, 7)) ;
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

        /// <summary>
        /// Gets the unsigned int32.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="firstBit">The first bit.</param>
        /// <param name="bitCount">The number of bits to read.</param>
        /// <returns>System.UInt32.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">bitCount must be in [0...31]</exception>
        public static uint GetUnsignedInt32(BitArray bits, int firstBit, int bitCount)
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

        /// <summary>
        /// Gets the unsigned int64.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="firstBit">The first bit.</param>
        /// <param name="bitCount">The number of bits to read.</param>
        /// <returns>System.UInt64.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">bitCount must be in [0...63]</exception>
        public static ulong GetUnsignedInt64(BitArray bits, int firstBit, int bitCount)
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
