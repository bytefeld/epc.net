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
    /// A SGTIN-96 Tag Uri
    /// </summary>
    public class Sgtin96Tag : SgtinTag
    {
        public const byte BinaryHeader = 0x30;

        public const string Scheme = "sgtin-96";
      
        /// <summary>
        /// Initializes a new instance of the <see cref="Sgtin96Tag" /> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="partition">The partition.</param>
        /// <param name="companyPrefix">The company prefix.</param>
        /// <param name="indicator">The indicator.</param>
        /// <param name="itemReference">The item reference.</param>
        /// <param name="serial">The serial.</param>
        public Sgtin96Tag(byte filter, byte partition, string companyPrefix, string indicator, string itemReference, string serial)
            : base(Scheme, filter, partition, companyPrefix, indicator, itemReference, serial)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sgtin96Tag" /> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="partition">The partition.</param>
        /// <param name="companyPrefix">The company prefix.</param>
        /// <param name="indicatorAnItemReference">The indicator an item reference.</param>
        /// <param name="serial">The serial.</param>
        public Sgtin96Tag(byte filter, byte partition, string companyPrefix, string indicatorAnItemReference, string serial)
            : base(Scheme, filter, partition, companyPrefix, indicatorAnItemReference, serial)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Sgtin96Tag"/> from the specified uri
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Sgtin96.</returns>
        public static new Sgtin96Tag FromUri(string uri)
        {
            return FromUri(EpcUri.FromString(uri));
        }

        /// <summary>
        /// Creates a new <see cref="Sgtin96Tag"/> from the specified uri
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Sgtin96.</returns>
        public static new Sgtin96Tag FromUri(EpcUri uri)
        {
            ValidateUri( uri, Scheme, 4);

            var parts = uri.Parts;
            byte filter = Byte.Parse(parts[0]);
            string companyPrefix = parts[1];
            string indicatorAnditemRef = parts[2];
            string serial = parts[3];
            
            byte partition = 0;
            switch(companyPrefix.Length)
            {
                case 12:
                    partition = 0;
                    break;
                case 11:
                    partition = 1;
                    break;
                case 10:
                    partition = 2;
                    break;
                case 9:
                    partition = 3;
                    break;
                case 8:
                    partition = 4;
                    break;
                case 7:
                    partition = 5;
                    break;
                case 6:
                    partition = 6;
                    break;
                default:
                    throw new FormatException("CompanyPrefix has invalid length.");
            }

            return new Sgtin96Tag(filter, partition, companyPrefix, indicatorAnditemRef, serial);
        }

        public static new Sgtin96Tag FromBinary(string epcText)
        {
            Assert.LengthIs("EpcCode", epcText, 24);

            BitArray bits = EpcEncoder.BinaryStringToBitArray(epcText);
            return FromBinary(bits);
        }

        public static Sgtin96Tag FromBinary(BitArray rawBits)
        {
            uint header = EpcEncoder.DecodeUInt32(rawBits, 0, 8);
            if (header != BinaryHeader)
                throw new FormatException(string.Format("Invalid EPC Header: 0x{0:X2} (expected 0x{1:X2)", header, BinaryHeader));

            string companyPrefix;
            string indicatorAnditemRef;
            ulong serial;
            byte partition;

            byte filter = (Byte)EpcEncoder.DecodeUInt32(rawBits, 8, 3);

            EpcEncoder.DecodePartition(rawBits, PartitionTable, 11, out partition, out companyPrefix, out indicatorAnditemRef);
            serial = EpcEncoder.ReadUInt64(rawBits, 58, 38);

            return new Sgtin96Tag(filter, partition, companyPrefix, indicatorAnditemRef, serial.ToString());
        }

        public override BitArray ToBitArray()
        {
            var bits = new BitArray(96);

            bits.Encode(BinaryHeader, 0, 8);
            bits.Encode(Filter, 8, 3);
            bits.EncodePartition(PartitionTable, 11, Partition, CompanyPrefix, IndicatorAndItemReference);
            bits.Encode(UInt64.Parse(Serial), 58, 38);

            return bits;
        }


    }
}
