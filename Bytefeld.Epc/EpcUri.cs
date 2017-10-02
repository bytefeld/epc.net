// 
// Copyright (c) 2017, Norbert Wagner (nw@bytefeld.com)
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bytefeld.Epc
{
    public enum EpcUriType
    {
        Unknown,
        Id,
        Tag
    }

    /// <summary>
    /// Represents an immutable EPC URI. 
    /// </summary>
    public class EpcUri
    {
        public const string EpcPrefix = "urn:epc";

        private readonly EpcUriType _type;
        private readonly string _scheme;
        private readonly string[] _parts;

        public EpcUri(EpcUriType type, string scheme, params string[] parts)
        {
            _type = type;
            _scheme = scheme;
            _parts = parts;
        }

        /// <summary>
        /// Gets the uri type (id or tag).
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public EpcUriType Type { get { return _type; } }

        /// <summary>
        /// Gets the URI scheme
        /// </summary>
        public string Scheme { get { return _scheme; } }

        /// <summary>
        /// Gets the scheme specific parts 
        /// </summary>
        public string[] Parts { get { return _parts; } }

        /// <summary>
        /// Creates a new EpcUri instance from the specified string representation.
        /// </summary>
        /// <param name="uriString">The URI string.</param>
        /// <returns>A new <see cref="EpcUri"/> instance</returns>
        /// <exception cref="System.FormatException">If the specified string is not parsable</exception>
        public static EpcUri FromString(string uriString)
        {
            string[] schemaParts = uriString.Split(':');
            if (schemaParts.Length != 5)
                throw new FormatException("URI must have the format 'urn:epc:<id|tag>:<scheme>:<part1>[.<part2>]...'");

            if (schemaParts[0] != "urn" || schemaParts[1] != "epc")
                throw new FormatException("URI must start with 'urn:epc:'");

            EpcUriType type;
            string typeStr = schemaParts[2];
            switch (typeStr)
            {
                case "id":
                    type = EpcUriType.Id;
                    break;
                case "tag":
                    type = EpcUriType.Tag;
                    break;
                default:
                    throw new FormatException("EPC URI type is invalid (must be 'id' or 'tag')");
            }

            string scheme = schemaParts[3];
            string[] parts = schemaParts[4].Split('.');

            return new EpcUri(type, scheme, parts);
        }

        /// <summary>
        /// Gets the URI string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}", EpcPrefix, _type.ToString().ToLower(), _scheme, string.Join(".", _parts));
        }

        public bool Equals(EpcUri uri)
        {
            if (uri == null)
                return false;

            return this.ToString().Equals(uri.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is EpcUri))
                return false;

            return this.Equals((EpcUri)obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }







    //internal struct EpcTagSpec : IEpcTagTraits
    //{
    //    public EpcTagSpec(EpcTagScheme scheme, Type epcTagClass, string uriSchemePart, byte binaryHeader, Func<string, EpcTag> textParser)
    //    {
    //        this.Schema = scheme;
    //        this.EpcTagClass = epcTagClass;
    //        this.UriSchemaPart = uriSchemePart;
    //        this.BinaryHeader = binaryHeader;
    //        this.TextParser = textParser;
    //    }

    //    public readonly Type EpcTagClass;
    //    public readonly EpcTagScheme Schema;
    //    public readonly string UriSchemaPart;
    //    public readonly byte BinaryHeader;
    //    public readonly Func<string, EpcTag> TextParser;

    //    public override int GetHashCode()
    //    {
    //        return this.Schema.GetHashCode();
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj == null || !(obj is EpcTagSpec))
    //            return false;

    //        return ((EpcTagSpec)obj).Schema.Equals(this.Schema);
    //    }

    //    #region IEpcTagTraits Members

    //    EpcTagScheme IEpcTagTraits.Scheme
    //    {
    //        get { return this.Schema; }
    //    }

    //    byte IEpcTagTraits.BinaryHeader
    //    {
    //        get { return this.BinaryHeader; }
    //    }

    //    string IEpcTagTraits.UriSchemaPart
    //    {
    //        get { return this.UriSchemaPart; }
    //    }

    //    #endregion
    //}

}
