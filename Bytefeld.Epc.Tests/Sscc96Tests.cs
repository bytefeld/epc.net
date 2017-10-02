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
    public class Sscc96Tests
    {


        [Test]
        [TestCase("3159542C8000000001000000", 6, "348338", "0000000001")]
        public void ParseBinaryTextSucceeds(string tagString, int partition, string companyPrefix, string serial )
        {

            Sscc96Tag tag = Sscc96Tag.FromBinary(tagString);

            Assert.AreEqual(partition, tag.Partition, "Partition invalid.");
            Assert.AreEqual(companyPrefix, tag.CompanyPrefix, "CompanyPrefix invalid.");
            Assert.AreEqual(serial, tag.Serial, "Serial invalid.");

        }

        [Test]
        public void FromUriSucceeds()
        {
            string uri = "urn:epc:tag:sscc-96:2.348338.90000000001";
            var tag = Sscc96Tag.FromUri(uri);

            Assert.AreEqual(2, tag.Filter, "Filter");
            Assert.AreEqual("348338", tag.CompanyPrefix, "CompanyPrefix");
            Assert.AreEqual("9", tag.Extension, "Extension");
            Assert.AreEqual("0000000001", tag.Serial, "Serial");
        }

        [Test]
        public void FromUriMatchesToUri()
        {
            string uri = "urn:epc:tag:sscc-96:2.348338.90000000001";
            var tag = Sscc96Tag.FromUri(uri);

            Assert.AreEqual(uri, tag.ToString(), "uri != tag.ToString()");
            Assert.AreEqual(uri, tag.ToUri().ToString(), "uri != tag.ToUri().ToString()");
        }
    }
}
