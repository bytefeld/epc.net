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
    public class Sgtin198Tests
    {


        [Test]
        [TestCase("3639542C82FB0258800000000000000000000000000000000000", 6, "348338", "0", "781321", "1")]
        [TestCase("3639542C82FB0258B266D1AB66EE1CB062C99B46AD9BB872C000", 6, "348338", "0", "781321", "12345678901234567890")]
        public void ParseBinaryTextSucceeds(string tagString, int partition, string companyPrefix, string indicator, string itemReference, string serial )
        {

            var tag = Sgtin198Tag.FromBinary(tagString);

            Assert.AreEqual(partition, tag.Partition, "Partition invalid.");
            Assert.AreEqual(companyPrefix, tag.CompanyPrefix, "CompanyPrefix invalid.");
            Assert.AreEqual(indicator, tag.Indicator, "Indicator invalid.");
            Assert.AreEqual(itemReference, tag.ItemReference, "ItemReference invalid.");
            Assert.AreEqual(serial, tag.Serial, "Serial invalid.");

        }

        [Test]
        public void FromUriSucceeds()
        {
            string uri = "urn:epc:tag:sgtin-198:1.348338.0781321.12345678901234567890";
            var tag = Sgtin198Tag.FromUri(uri);

            Assert.AreEqual(1, tag.Filter, "Filter");
            Assert.AreEqual("348338", tag.CompanyPrefix, "CompanyPrefix");
            Assert.AreEqual("0781321", tag.IndicatorAndItemReference, "IndicatorAndItemReference");
            Assert.AreEqual("0", tag.Indicator, "Indicator");
            Assert.AreEqual("781321", tag.ItemReference, "ItemReference");
            Assert.AreEqual("12345678901234567890", tag.Serial, "Serial");
        }

        [Test]
        public void FromUriMatchesToUri()
        {
            string uri = "urn:epc:tag:sgtin-198:1.348338.0781321.12345678901234567890";
            var tag = Sgtin198Tag.FromUri(uri);

            Assert.AreEqual(uri, tag.ToString(), "uri != tag.ToString()");
            Assert.AreEqual(uri, tag.ToUri().ToString(), "uri != tag.ToUri().ToString()");
        }
    }
}
