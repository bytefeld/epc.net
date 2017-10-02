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
    /// Common base class for SGTIN tags (SGTIN-96 and SGTIN-198)
    /// </summary>
    public abstract class SgtinTag : EpcTag, ISgtin
    {
        private readonly string _scheme;
        private readonly string _companyPrefix;
        private readonly string _indicator;
        private readonly string _itemReference;
        private readonly string _serial;
        private readonly byte _filter;
        private readonly byte _partition;

        protected SgtinTag(string scheme, byte filter, byte partition, string companyPrefix, string indicator, string itemReference, string serial)
        {
            _scheme = scheme;
            _filter = filter;
            _partition = partition;
            _indicator = indicator;
            _companyPrefix = companyPrefix;
            _itemReference = itemReference;
            _serial = serial;
        }

        protected SgtinTag(string scheme, byte filter, byte partition, string companyPrefix, string indicatorAnItemReference, string serial)
        {
            _scheme = scheme;
            _filter = filter;
            _partition = partition;
            _indicator = indicatorAnItemReference.Substring(0, 1);
            _companyPrefix = companyPrefix;
            _itemReference = indicatorAnItemReference.Substring(1);
            _serial = serial;
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
        /// Gets the indicator.
        /// </summary>
        /// <value>
        /// The indicator.
        /// </value>
        public string Indicator { get { return _indicator; } }

        /// <summary>
        /// Gets the combined indicator and item reference.
        /// </summary>
        /// <value>The indicator and item reference.</value>
        public string IndicatorAndItemReference { get { return _indicator + _itemReference; } }

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
        /// Gets the item reference.
        /// </summary>
        /// <value>
        /// The item reference.
        /// </value>
        public string ItemReference { get { return _itemReference; } }

        /// <summary>
        /// Gets the corresponding <see cref="SgtinID"/> pure ID.
        /// </summary>
        /// <returns>The EPC pure SGTIN id</returns>
        public SgtinID ToSgtin()
        {
            return new SgtinID(this.CompanyPrefix, this.Indicator, this.ItemReference, this.Serial);
        }


        public override EpcUri ToUri()
        {
            return new EpcUri(EpcUriType.Tag, _scheme, Filter.ToString(), CompanyPrefix, IndicatorAndItemReference, Serial);
        }

        internal static EpcEncoder.Partition[] PartitionTable = new EpcEncoder.Partition[] 
        {
            new EpcEncoder.Partition() { Bits1 = 40, Digits1=12, Bits2=04, Digits2=1 },
            new EpcEncoder.Partition() { Bits1 = 37, Digits1=11, Bits2=07, Digits2=2 },
            new EpcEncoder.Partition() { Bits1 = 34, Digits1=10, Bits2=10, Digits2=3 },
            new EpcEncoder.Partition() { Bits1 = 30, Digits1=09, Bits2=14, Digits2=4 },
            new EpcEncoder.Partition() { Bits1 = 27, Digits1=08, Bits2=17, Digits2=5 },
            new EpcEncoder.Partition() { Bits1 = 24, Digits1=07, Bits2=20, Digits2=6 },
            new EpcEncoder.Partition() { Bits1 = 20, Digits1=06, Bits2=24, Digits2=7 }
        };

      
    }

}
