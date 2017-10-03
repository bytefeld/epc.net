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
    public class EpcTagsTests
    {
        [Test]
        public void ConvertBinaryToTypedTag()
        {
            var tag = EpcTag.FromBinary<Sgtin96Tag>("302D28B329B0F6C000000001");

            Assert.NotNull(tag);
            Assert.AreEqual("urn:epc:tag:sgtin-96:1.311112347.0987.1", tag.ToString());
        }

        [Test]
        [TestCase("302D28B329B0F6C000000001", "urn:epc:tag:sgtin-96:1.311112347.0987.1", "urn:epc:id:sgtin:311112347.0987.1")]
        [TestCase("3639542C82FB0258B266D1AB66EE1CB062C99B46AD9BB872C000", "urn:epc:tag:sgtin-198:1.348338.0781321.12345678901234567890", "urn:epc:id:sgtin:348338.0781321.12345678901234567890")]
        [TestCase("3159542C8000000001000000", "urn:epc:tag:sscc-96:2.348338.00000000001", "urn:epc:id:sscc:348338.00000000001")]
        public void ConvertBinariesToUris(string binaryTag, string tagUri, string idUri)
        {
            var uri = EpcTag.FromBinary(binaryTag).ToString();
            Assert.AreEqual(tagUri, uri);
        }

        [Test]
        [TestCase("302D28B329B0F6C000000001", "urn:epc:tag:sgtin-96:1.311112347.0987.1")]
        [TestCase("3639542C82FB0258B266D1AB66EE1CB062C99B46AD9BB872C000", "urn:epc:tag:sgtin-198:1.348338.0781321.12345678901234567890")]
        [TestCase("3159542C8000000001000000", "urn:epc:tag:sscc-96:2.348338.00000000001")]
        public void ConvertUrisToBinaries(string expectedTag, string uri)
        {
            string hex = EpcTag.FromUri(uri).ToBinary();
            Assert.AreEqual(expectedTag, hex);
        }
    }
}
