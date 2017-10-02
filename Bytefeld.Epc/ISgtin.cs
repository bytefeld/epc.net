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
    /// Provides common SGTIN properties that are provided by pure SGTIN ids and SGTIN-xx tags.
    /// </summary>
    public interface ISgtin
    {
        string CompanyPrefix { get; }

        string Indicator { get; }

        string ItemReference { get; }

        string Serial { get; }
    }


    /// <summary>
    /// Common extensions for <see cref="ISgtn"/>
    /// </summary>
    public static class SgtinExtensions {

        public static string GetEan13(this ISgtin me)
        {
            return Gtin13.Create(me.CompanyPrefix, me.ItemReference);
        }
    }
}
