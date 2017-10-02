// 
// Copyright (c) 2017, Norbert Wagner (nw@bytefeld.com)
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bytefeld.Epc.Tests
{

    [TestFixture]
    public class EpcUriTests
    {

        [Test]
        public void EpcUri_ToStringIsParsableByFromString()
        {
            EpcUri uri = new EpcUri(EpcUriType.Tag, "sgtin-96", "a", "b", "c");

            string str = uri.ToString();
            EpcUri uri2 = EpcUri.FromString(str);

            Assert.AreEqual(uri, uri2);
        }
    }
}
