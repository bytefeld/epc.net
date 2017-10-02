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
    /// Represents a EPC Serial Shipping Container Code (SSCC) pure ID
    /// </summary>
    public class SsccId : EpcId, ISscc
    {
        private readonly string _companyPrefix;
        private readonly string _extension;
        private readonly string _serial;

        public const string Scheme = "sscc";

        /// <summary>
        /// Initializes a new instance of the <see cref="SsccId" /> class.
        /// </summary>
        /// <param name="companyPrefix">The company prefix.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="serial">The serial.</param>
        public SsccId(string companyPrefix, string extension, string serial)
        {
            _companyPrefix = companyPrefix;
            _extension = extension;
            _serial = serial;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SsccId" /> class.
        /// </summary>
        /// <param name="companyPrefix">The company prefix.</param>
        /// <param name="extensionAndSerial">The extension and serial.</param>
        public SsccId(string companyPrefix, string extensionAndSerial)
        {
            _companyPrefix = companyPrefix;
            _extension = extensionAndSerial.Substring(0,1);
            _serial = extensionAndSerial.Substring(1);
        }

        /// <summary>
        /// Gets the company prefix.
        /// </summary>
        /// <value>
        /// The company prefix.
        /// </value>
        public string CompanyPrefix { get { return _companyPrefix; } }

        /// <summary>
        /// Gets the serial number
        /// </summary>
        /// <value>
        /// The serial.
        /// </value>
        public string Serial { get { return _serial; } }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <value>The extension.</value>
        public string Extension { get { return _extension; } }

        /// <summary>
        /// Gets the extension and serial.
        /// </summary>
        /// <value>The extension and serial.</value>
        public string ExtensionAndSerial { get { return _extension + _serial; } }

        /// <summary>
        /// Converts the SSCC to its URI representation
        /// </summary>
        /// <returns>EpcUri.</returns>
        public override EpcUri ToUri()
        {
            return new EpcUri(EpcUriType.Id, Scheme, CompanyPrefix, ExtensionAndSerial);
        }
    }


}
