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

    /// <summary>
    /// The test main program. Used to ease test debugging 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            //var tests = new EpcTagsTests();
            //tests.ConvertBinariesToUris("302D28B329B0F6C000000001", "epc:tag:sgtin-96:1.311112347.0987.1");

            var sgtin198Test = new Sgtin198Tests();
            sgtin198Test.ParseBinaryTextSucceeds("3639542C82FB0258B266D1AB66EE1CB062C99B46AD9BB872C000", 6, "348338", "0", "781321", "12345678901234567890");

            var sscc96Test = new Sscc96Tests();
            sscc96Test.FromUriMatchesToUri();
        }
    }
}
