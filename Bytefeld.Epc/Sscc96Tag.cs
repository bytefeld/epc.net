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
    /// A SSCC-96 Tag Uri (Serial Shipping Container Code)
    /// </summary>
    public class Sscc96Tag : EpcTag, ISscc
    {
        public const byte BinaryHeader = 0x31;

        public const string Scheme = "sscc-96";

        private readonly string _companyPrefix;
        private readonly string _extension;
        private readonly string _serial;
        private readonly byte _filter;
        private readonly byte _partition;

        /// <summary>
        /// Gets the partition.
        /// </summary>
        /// <value>
        /// The partition.
        /// </value>
        public byte Partition { get { return _partition; } }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public byte Filter { get { return _filter; } }

        /// <summary>
        /// Gets the company prefix.
        /// </summary>
        /// <value>
        /// The company prefix.
        /// </value>
        public string CompanyPrefix { get { return _companyPrefix; } }

        /// <summary>
        /// Gets the serial.
        /// </summary>
        /// <value>
        /// The serial.
        /// </value>
        public string Serial { get { return _serial; } }

        /// <summary>
        /// Gets the extension
        /// </summary>
        public string Extension { get { return _extension; } }

        /// <summary>
        /// Gets the combined extension and serial
        /// </summary>
        public string ExtensionAndSerial { get { return _extension + _serial; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sgtin96Tag" /> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="partition">The partition.</param>
        /// <param name="companyPrefix">The company prefix.</param>
        /// <param name="extensionAndSerial">The extension and serial.</param>
        public Sscc96Tag(byte filter, byte partition, string companyPrefix, string extensionAndSerial)
        {
            _filter = filter;
            _partition = partition;
            _companyPrefix = companyPrefix;
            _extension = extensionAndSerial.Substring(0,1);
            _serial = extensionAndSerial.Substring(1);
        }

        public Sscc96Tag(byte filter, byte partition, string companyPrefix, string extension, string serial)
        {
            _filter = filter;
            _partition = partition;
            _companyPrefix = companyPrefix;
            _extension = extension;
            _serial = serial;
        }

        /// <summary>
        /// Creates a new <see cref="Sgtin96Tag"/> from the specified uri
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Sgtin96.</returns>
        public new static Sscc96Tag FromUri(string uri)
        {
            return FromUri(EpcUri.FromString(uri));
        }

        /// <summary>
        /// Creates a new <see cref="Sgtin96Tag"/> from the specified uri
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Sgtin96.</returns>
        public new static Sscc96Tag FromUri(EpcUri uri)
        {
            ValidateUri( uri, Scheme, 3);

            var parts = uri.Parts;
            byte filter = Byte.Parse(parts[0]);
            string companyPrefix = parts[1];
            string extensionAndSerial = parts[2];
            
            var partition = GetBestPartitionNumber(companyPrefix.Length);

            return new Sscc96Tag(filter, partition, companyPrefix, extensionAndSerial);
        }

       
        public new static Sscc96Tag FromBinary(string epcCode)
        {
            Assert.LengthIs("EpcCode", epcCode, 24);

            BitArray bits = EpcEncoder.BinaryStringToBitArray(epcCode);
            return FromBinary(bits);
        }

        public static Sscc96Tag FromBinary(BitArray rawBits)
        {
            uint header = rawBits.DecodeUInt32(0, 8);
            if (header != BinaryHeader)
                throw new FormatException(string.Format("Invalid EPC Header: 0x{0:X2} (expected 0x{1:X2)", header, BinaryHeader));

            string companyPrefix;
            string extensionAndSerial;
            byte partition;

            byte filter = rawBits.DecodeByte(8, 3);

            rawBits.DecodePartition(PartitionTable, 11, out partition, out companyPrefix, out extensionAndSerial);

            return new Sscc96Tag(filter, partition, companyPrefix, extensionAndSerial);
        }

        public override BitArray ToBitArray()
        {
            var bits = new BitArray(96);

            bits.Encode(BinaryHeader, 0, 8);
            bits.Encode(_filter, 8, 3);
            bits.EncodePartition(PartitionTable, 11, _partition, _companyPrefix, ExtensionAndSerial);

            return bits;
        }

        /// <summary>
        /// Gets the corresponding <see cref="SsccId"/> pure ID.
        /// </summary>
        /// <returns>The EPC pure SGTIN id</returns>
        public SsccId ToSsccId()
        {
            return new SsccId(this.CompanyPrefix, this.ExtensionAndSerial);
        }

        public override EpcUri ToUri()
        {
            return new EpcUri(EpcUriType.Tag, Scheme, Filter.ToString(), CompanyPrefix, ExtensionAndSerial);
        }

       

        /// <summary>
        /// Gets the best partition number for the specified company prefix length
        /// </summary>
        /// <param name="companyPrefixLength"></param>
        /// <returns></returns>
        private static byte GetBestPartitionNumber(int companyPrefixLength)
        {
            byte partition = 0;
            switch (companyPrefixLength)
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
            return partition;
        }

        private static EpcEncoder.Partition[] PartitionTable = new EpcEncoder.Partition[] 
        {
            new EpcEncoder.Partition() { Bits1 = 40, Digits1=12, Bits2=18, Digits2=05 },
            new EpcEncoder.Partition() { Bits1 = 37, Digits1=11, Bits2=21, Digits2=06 },
            new EpcEncoder.Partition() { Bits1 = 34, Digits1=10, Bits2=24, Digits2=07 },
            new EpcEncoder.Partition() { Bits1 = 30, Digits1=09, Bits2=28, Digits2=08 },
            new EpcEncoder.Partition() { Bits1 = 27, Digits1=08, Bits2=31, Digits2=09 },
            new EpcEncoder.Partition() { Bits1 = 24, Digits1=07, Bits2=34, Digits2=10 },
            new EpcEncoder.Partition() { Bits1 = 20, Digits1=06, Bits2=38, Digits2=11 }
        };
    }
}
