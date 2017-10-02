// 
// Copyright (c) 2017, Norbert Wagner (nw@bytefeld.com)
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bytefeld.Epc
{
    /// <summary>
    /// An EPC "pure identity URI". Main API to work with EPC ID URIs
    /// </summary>
    public abstract class EpcId
    {

        /// <summary>
        /// To the URI.
        /// </summary>
        /// <returns>EpcUri.</returns>
        public abstract EpcUri ToUri();

        /// <summary>
        /// Create a new <see cref="EpcId"/> from the specified uri
        /// </summary>
        /// <param name="epcIdUri">The epc id URI.</param>
        /// <returns>the created <see cref="EpcId"/>.</returns>
        public static EpcId FromUri(string epcIdUri)
        {
            EpcUri uri = EpcUri.FromString(epcIdUri);
            return FromUri(uri);
        }

        /// <summary>
        /// Create a new <see cref="EpcId"/> from the specified uri
        /// </summary>
        /// <param name="epcIdUri">The epc id URI.</param>
        /// <returns>the created <see cref="EpcId"/>.</returns>
        public static EpcId FromUri(EpcUri uri)
        {
            if (uri.Type != EpcUriType.Id)
                throw new FormatException("Invalid uri type.");

            switch( uri.Scheme)
            {
                case SgtinId.Scheme:
                    return SgtinId.FromUri(uri);
            }

            throw new FormatException("Invalid uri scheme.");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance. Same as <c>ToUri().ToString()</c>
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToUri().ToString();
        }

    }
}
