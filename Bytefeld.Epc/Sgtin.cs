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
    /// A SGTIN Uri
    /// </summary>
    public class Sgtin : EpcId, ISgtin
    {
        private readonly string _companyPrefix;
        private readonly string _itemReference;
        private readonly string _indicator;
        private readonly string _serial;

        public const string Scheme = "sgtin";
        
        public Sgtin(string companyPrefix, string indicator, string itemReference, string serial)
        {
            _companyPrefix = companyPrefix;
            _indicator = indicator;
            _itemReference = itemReference;
            _serial = serial;
        }

         /// <summary>
        /// Create a new <see cref="Sgtin"/> from the specified uri
        /// </summary>
        /// <param name="epcIdUri">The epc id URI.</param>
        /// <returns>the created <see cref="Sgtin"/>.</returns>
        public static new Sgtin FromUri(string epcIdUri)
        {
            EpcUri uri = EpcUri.FromString(epcIdUri);
            return FromUri(uri);
        }

        /// <summary>
        /// Create a new <see cref="Sgtin"/> from the specified uri
        /// </summary>
        /// <param name="epcIdUri">The epc id URI.</param>
        /// <returns>the created <see cref="Sgtin"/>.</returns>
        public static new Sgtin FromUri(EpcUri uri)
        {
            string indicator = uri.Parts[1].Substring(0,1);
            string itemRef = uri.Parts[1].Substring(1);
            
            return new Sgtin(uri.Parts[0], indicator, itemRef, uri.Parts[2]);
        }

        /// <summary>
        /// Gets the company prefix.
        /// </summary>
        /// <value>
        /// The company prefix.
        /// </value>
        public string CompanyPrefix { get { return _companyPrefix; } }

        /// <summary>
        /// Gets the indicator.
        /// </summary>
        /// <value>
        /// The indicator.
        /// </value>
        public string Indicator { get { return _indicator; } }

        /// <summary>
        /// Gets the item reference.
        /// </summary>
        /// <value>
        /// The item reference.
        /// </value>
        public string ItemReference { get { return _itemReference; } }

        /// <summary>
        /// Gets the combined indicator and item reference.
        /// </summary>
        /// <value>The indicator and item reference.</value>
        public string IndicatorAndItemReference { get { return _indicator + _itemReference; } }

        /// <summary>
        /// Gets the serial.
        /// </summary>
        /// <value>
        /// The serial.
        /// </value>
        public string Serial { get { return _serial; } }


        public override EpcUri ToUri()
        {
            return new EpcUri(EpcUriType.Id, Scheme, CompanyPrefix, Indicator + ItemReference, Serial);
        }
    }


}
