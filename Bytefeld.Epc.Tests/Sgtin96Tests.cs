// 
// Copyright (c) 2017, Norbert Wagner (nw@bytefeld.com)
//
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Bytefeld.Epc;

namespace Bytefeld.Epc.Tests
{
    [TestFixture]
    public class Sgtin96Tests
    {


        [Test]
        [TestCase("3039542C82FB024000000001", 6, "348338", "0", "781321", "3483387813215")]
        [TestCase("3034C8470C0796C000000001", 5, "3281347", "0", "07771", "3281347077710")]
        [TestCase("3030F380DE001EC000000001", 4, "31916476", "0", "0123", "3191647601235")]
        [TestCase("302D28B329B0F6C000000001", 3, "311112347", "0", "987", "3111123479875")]
        [TestCase("3028B3A7F7390E0000000001", 2, "3014129465", "0", "56", "3014129465562")]
        [TestCase("3024DFD31D1DE0C000000001", 1, "30041237743", "0", "3", "3004123774333")]
        [TestCase("302117688465380000000001", 0, "300012345678", "0", "", "3000123456781")]
        public void ParseBinaryTextSucceeds(string tagString, int partition, string companyPrefix, string indicator, string itemReference, string ean13 )
        {

            Sgtin96Tag tag = Sgtin96Tag.FromBinary(tagString);

            Assert.AreEqual(partition, tag.Partition, "Partition invalid.");
            Assert.AreEqual(companyPrefix, tag.CompanyPrefix, "CompanyPrefix invalid.");
            Assert.AreEqual(indicator, tag.Indicator, "Indicator invalid.");
            Assert.AreEqual(itemReference, tag.ItemReference, "ItemReference invalid.");
            Assert.AreEqual(ean13, tag.GetEan13(), "Ean13 invalid.");

        }

        [Test]
        public void FromUriSucceeds()
        {
            string uri = "urn:epc:tag:sgtin-96:1.311112347.0781321.1";
            var tag = Sgtin96Tag.FromUri(uri);

            Assert.AreEqual(1, tag.Filter, "Filter");
            Assert.AreEqual("311112347", tag.CompanyPrefix, "CompanyPrefix");
            Assert.AreEqual("0781321", tag.IndicatorAndItemReference, "IndicatorAndItemReference");
            Assert.AreEqual("0", tag.Indicator, "Indicator");
            Assert.AreEqual("781321", tag.ItemReference, "ItemReference");
            Assert.AreEqual("1", tag.Serial, "Serial");
            Assert.AreEqual(uri, tag.ToUri().ToString(), "uri != tag.ToUri().ToString()");
        }

        [Test]
        public void FromUriMatchesToUri()
        {
            string uri = "urn:epc:tag:sgtin-96:1.311112347.0781321.1";
            var tag = Sgtin96Tag.FromUri(uri);

            Assert.AreEqual(uri, tag.ToString(), "uri != tag.ToString()");
            Assert.AreEqual(uri, tag.ToUri().ToString(), "uri != tag.ToUri().ToString()");
        }
    }
}
