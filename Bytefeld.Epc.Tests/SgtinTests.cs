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
    public class SgtinTests
    {

        [Test]
        public void FromUriSucceeds()
        {
            string uri = "urn:epc:id:sgtin:311112347.0781321.1";
            var id = SgtinId.FromUri(uri);

            Assert.AreEqual("311112347", id.CompanyPrefix, "CompanyPrefix");
            Assert.AreEqual("0781321", id.IndicatorAndItemReference, "IndicatorAndItemReference");
            Assert.AreEqual("0", id.Indicator, "Indicator");
            Assert.AreEqual("781321", id.ItemReference, "ItemReference");
            Assert.AreEqual("1", id.Serial, "Serial");
        }

        [Test]
        public void FromUriMatchesToUri()
        {
            string uri = "urn:epc:id:sgtin:311112347.0781321.1";
            var id = SgtinId.FromUri(uri);

            Assert.AreEqual(uri, id.ToString(), "uri != id.ToString()");
            Assert.AreEqual(uri, id.ToUri().ToString(), "uri != id.ToUri().ToString()");
        }
    }
}
