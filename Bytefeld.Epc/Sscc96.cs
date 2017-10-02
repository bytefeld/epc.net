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
    public class Sscc96 : EpcTag, ISscc
    {
        public const byte BinaryHeader = 0x31;

        public const string Scheme = "sscc-96";

        private readonly string _companyPrefix;
        private readonly string _extension;
        private readonly string _serial;
        private readonly byte _filter;
        private readonly byte _partition;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sgtin96" /> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="partition">The partition.</param>
        /// <param name="companyPrefix">The company prefix.</param>
        /// <param name="extensionAndSerial">The extension and serial.</param>
        public Sscc96(byte filter, byte partition, string companyPrefix, string extensionAndSerial)
        {
            _filter = filter;
            _partition = partition;
            _companyPrefix = companyPrefix;
            _extension = extensionAndSerial.Substring(0,1);
            _serial = extensionAndSerial.Substring(1);
        }

        public Sscc96(byte filter, byte partition, string companyPrefix, string extension, string serial)
        {
            _filter = filter;
            _partition = partition;
            _companyPrefix = companyPrefix;
            _extension = extension;
            _serial = serial;
        }

        /// <summary>
        /// Creates a new <see cref="Sgtin96"/> from the specified uri
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Sgtin96.</returns>
        public static new Sscc96 FromUri(string uri)
        {
            return FromUri(EpcUri.FromString(uri));
        }

        /// <summary>
        /// Creates a new <see cref="Sgtin96"/> from the specified uri
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>Sgtin96.</returns>
        public static new Sscc96 FromUri(EpcUri uri)
        {
            ValidateUri( uri, Scheme, 3);

            var parts = uri.Parts;
            byte filter = Byte.Parse(parts[0]);
            string companyPrefix = parts[1];
            string extensionAndSerial = parts[2];
            
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

            return new Sscc96(filter, partition, companyPrefix, extensionAndSerial);
        }

        public static new Sscc96 FromBinary(string epcCode)
        {
            Assert.LengthIs("EpcCode", epcCode, 24);

            BitArray bits = EpcEncoder.ConvertToBitArray(epcCode);
            return FromBinary(bits);
        }

        public static Sscc96 FromBinary(BitArray rawBits)
        {
            uint header = EpcEncoder.GetUnsignedInt32(rawBits, 0, 8);
            if (header != BinaryHeader)
                throw new FormatException(string.Format("Invalid EPC Header: 0x{0:X2} (expected 0x{1:X2)", header, BinaryHeader));

            string companyPrefix;
            string extenstionAndSerial;
            byte partition;

            byte filter = (Byte)EpcEncoder.GetUnsignedInt32(rawBits, 8, 3);

            EpcEncoder.ParsePartitionTable(rawBits, 11, PartitionTable, out partition, out companyPrefix, out extenstionAndSerial);

            return new Sscc96(filter, partition, companyPrefix, extenstionAndSerial);
        }

        /// <summary>
        /// Gets the corresponding <see cref="Sscc"/> pure ID.
        /// </summary>
        /// <returns>The EPC pure SGTIN id</returns>
        public Sscc ToSscc()
        {
            return new Sscc(this.CompanyPrefix, this.ExtensionAndSerial);
        }

        public override EpcUri ToUri()
        {
            return new EpcUri(EpcUriType.Tag, Scheme, Filter.ToString(), CompanyPrefix, ExtensionAndSerial);
        }

        public override string ToBinary()
        {
            throw new NotImplementedException();
        }

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

        public string Extension { get { return _extension; } }

        public string ExtensionAndSerial { get { return _extension + _serial; } }


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
