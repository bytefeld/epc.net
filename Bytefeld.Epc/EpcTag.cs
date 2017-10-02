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

/*
 * 0000 0001 
        0000 001x 
        0000 01xx 
        01 
        02,03 
        04,05 
        06,07 
        NA 
        NA 
        NA 
        NA 
        Reserved for Future Use 
        Reserved for Future Use 
        Reserved for Future Use 
        Reserved for Future Use 
        0000 1000  08  64  Reserved until 64bit Sunset <SSCC-64> 
        0000 1001  09  64  Reserved until 64bit Sunset <SGLN-64> 
        0000 1010  0A  64  Reserved until 64bit Sunset <GRAI-64> 
        0000 1011  0B  64  Reserved until 64bit Sunset <GIAI-64> 
        0000 1100 to 0000 1111 0C to 0F   Reserved until 64 bit Sunset Due to 64 bit encoding rule in Gen 1 
        0001 0000 to 0010 1011 10 to 2B NA NA Reserved for Future Use 
        0010 1100  2C  96  GDTI-96 
        0010 1101  2D  96  GSRN-96 
        0010 1110  2E  NA  Reserved for Future Use 
        0010 1111  2F  96  USDoD-96 
        0011 0000  30  96  SGTIN-96 
        0011 0001  31  96  SSCC-96 
        0011 0010  32  96  SGLN-96 
        0011 0011  33  96  GRAI-96 
        0011 0100  34  96  GIAI-96 
        0011 0101  35  96  GID-96 
        0011 0110  36  198  SGTIN-198 
        0011 0111  37  170  GRAI-170 
        0011 1000  38  202  GIAI-202 
        0011 1001  39  195  SGLN-195 
        0011 1010  3A  113  GDTI-113 
        0011 1011  3B  Variable  ADI-var 
 */
    /// <summary>
    /// Base for all EPC tag classes and universal EPC tag interface
    /// </summary>
    public abstract class EpcTag 
    {

        /// <summary>
        /// Creates an <see cref="EpcTag" /> from the specified binary representation
        /// </summary>
        /// <param name="epcHexText">The epc tag encoded in hexadecimal text.</param>
        /// <returns>EpcTag.</returns>
        /// <exception cref="System.FormatException"></exception>
        public static EpcTag FromBinary(string epcHexText)
        {

            byte header = Byte.Parse(epcHexText.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);

            Func<string, EpcTag> parser = null;
            switch (header)
            {
                case Sgtin96Tag.BinaryHeader:
                    parser = Sgtin96Tag.FromBinary;
                    break;
                case Sgtin198Tag.BinaryHeader:
                    parser = Sgtin198Tag.FromBinary;
                    break;
                case Sscc96Tag.BinaryHeader:
                    parser = Sscc96Tag.FromBinary;
                    break;
                default:
                    throw new FormatException(string.Format("Invalid binary header value: 0x{0:X2}", header));
            }
            return parser(epcHexText);
        }

        /// <summary>
        /// Creates a <see cref="EpcTag" /> of type <typeparam name="TTag"/> from the specified binary representation
        /// </summary>
        /// <param name="epcHexText">The epc tag encoded in hexadecimal text.</param>
        /// <returns>EpcTag.</returns>
        /// <exception cref="System.FormatException">The specified text has a wrong format or is not of type <typeparam name="TTag"/></exception>
        public static TTag FromBinary<TTag>(string epcHexText) where TTag : EpcTag
        {
            EpcTag tag = FromBinary(epcHexText);

            TTag typedTag = tag as TTag;
            if (typedTag == null)
            {
                throw new FormatException(string.Format("Invalid tag type: {0}", tag.GetType().Name));
            }

            return typedTag;
        }


        /// <summary>
        /// Creates an <see cref="EpcTag"/> from the specified EPC Tag URI
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>EpcTag.</returns>
        public static EpcTag FromUri(string uri)
        {
            return FromUri(EpcUri.FromString(uri));
        }

        /// <summary>
        /// Creates an <see cref="EpcTag"/> from the specified EPC Tag URI
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>EpcTag.</returns>
        public static EpcTag FromUri(EpcUri uri)
        {
            ValidateUri(uri);
            switch (uri.Scheme)
            {
                case Sgtin96Tag.Scheme:
                    return Sgtin96Tag.FromUri(uri);
                case Sgtin198Tag.Scheme:
                    return Sgtin198Tag.FromUri(uri);
                case Sscc96Tag.Scheme:
                    return Sscc96Tag.FromUri(uri);
                default:
                    throw new NotSupportedException(string.Format("EPC tag uri scheme is not supported: {0}", uri.Scheme));
            }
        }

        /// <summary>
        /// Converts the <see cref="EpcTag"/> to a <see cref="EpcUri"/>
        /// </summary>
        /// <returns>EpcUri.</returns>
        abstract public EpcUri ToUri();

        /// <summary>
        /// Gets the <see cref="EpcTag"/>'s binary text representation 
        /// </summary>
        /// <returns>System.String.</returns>
        abstract public string ToBinary();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance. Same as <c>ToUri().ToString()</c>
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToUri().ToString();
        }

        /// <summary>
        /// Ensures that an URI matches specified criteria
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="expectedScheme">The expected scheme.</param>
        /// <param name="partCount">The part count.</param>
        /// <exception cref="System.FormatException">The specified uri is invalid.</exception>
        protected static void ValidateUri(EpcUri uri, string expectedScheme = null, int partCount = -1)
        {
            if (uri.Type != EpcUriType.Tag)
                throw new FormatException("EPC uri type is invalid.");

            if (expectedScheme != null && uri.Scheme != expectedScheme)
                throw new FormatException(string.Format("EPC tag uri scheme is invalid (expected: {0}, found: {1})", expectedScheme, uri.Scheme));

            if (partCount > 0 && uri.Parts.Length != partCount)
                throw new FormatException(string.Format("EPC {0} uri has invalid number of parts (expected {1}, found {2}).", uri.Scheme, partCount, uri.Parts.Length));
        }
    }
}
