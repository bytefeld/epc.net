// 
// Copyright (c) 2017, Norbert Wagner (nw@bytefeld.com)
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bytefeld.Epc
{
    public static class Gtin13
    {

        public static string Create(string companyPrefix, string itemReference)
        {

            string payload = companyPrefix + itemReference;
            int check = CalculateCheckDigit(payload);

            return payload + check.ToString();
        }


        public static int CalculateCheckDigit(string payload)
        {

            payload = "0" + payload;

            int evens = 0;
            int odds = 0;
            for( int i = 0; i<payload.Length; i++)
            {
                int b = (byte)payload[i] - (byte)'0';
                if ((i & 1) == 0)
                {
                    odds += b;
                }
                else
                {
                    evens += b;
                }
            }

            int check = 10 - (((3 * odds) + evens) % 10);
            return Math.Abs(check) % 10;
        }

    }
}
