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
    /// Provides common SSCC properties that are provided by pure SSCC ids and SSCC-96 tags.
    /// </summary>
    public interface ISscc
    {
        string CompanyPrefix { get; }

        string ExtensionAndSerial { get; }

        string Extension { get; }

        string Serial { get; }
    }


  
}
