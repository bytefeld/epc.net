using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bytefeld.Epc
{
    static class Assert
    {
        /// <summary>
        /// Asserts that the specified instance is not null
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="instance">The instance.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void NotNull(string name, object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Asserts that the length a string matches exactly a specified value
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="str">The STR.</param>
        /// <param name="length">The length.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.FormatException"></exception>
        public static void LengthIs(string name, string str, int length)
        {
            if (str == null)
            {
                throw new ArgumentNullException(name);
            }
            if (str.Length != length)
            {
                throw new FormatException(string.Format("{0} must be {1} characters long (was {2}).", name, length, str.Length));
            }
        }

        public static void OneOf<T>(string name, T val, params T[] allowedValues)
        {
            if (!allowedValues.Any(v => v.Equals(val)))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("{0} must be one of [{1}] but was {2}.", name, string.Join(",", allowedValues.Select(v => v.ToString()).ToArray()), val));
            }
        }

        public static void InRange<T>(string name, T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0 || val.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(
                      string.Format("{0} must be in [{1}..{2}] but was {3}.", name, min, max, val));
            }
        }

        public static void MultipleOf(string name, int val, int multiplicator)
        {
            if ((val % multiplicator) != 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} must be a multiple of {1} (was {2}).", name, multiplicator, val));
            }
        }
    }
}

